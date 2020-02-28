using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Hbm.Api.Common;
using Hbm.Api.Common.Messaging;
using Hbm.Api.Common.Exceptions;
using Hbm.Api.Common.Entities;
using Hbm.Api.Common.Entities.Problems;
using Hbm.Api.Common.Entities.Connectors;
using Hbm.Api.Common.Entities.Channels;
using Hbm.Api.Common.Entities.Signals;
using Hbm.Api.Common.Entities.Filters;
using Hbm.Api.Common.Entities.ConnectionInfos;
using Hbm.Api.Common.Enums;
using Hbm.Api.Scan;
using Hbm.Api.Pmx;
using Hbm.Api.QuantumX;
using Hbm.Api.Mgc;

namespace HMB_Utility
{
    public class DAQ
    {
        Device measDevice = null;
        public delegate void SaveDataFromDaq(Signal sig); //delegate to the method that describe how to save the data from DAQ
        SaveDataFromDaq saveDataMethod;
        public event EventHandler<string> warningEvent;
        System.Timers.Timer dataFetchTimer = null;
        DaqMeasurement daqSession = null;
        List<Problem> daqPrepareProblems = null;
        List<Signal> signalsToMeasure = null;


        public DAQ(FoundDevice dev, SaveDataFromDaq saveMethod)
        {
            daqPrepareProblems = new List<Problem>();
            daqSession = new DaqMeasurement();
            signalsToMeasure = dev.signalsToMeas;
            saveDataMethod = saveMethod;
            measDevice = dev.device;
        }

        ~DAQ()
        {
            daqSession.Dispose();
        }


        private void Start(int fetchPeriod = 500, decimal sampleRate = 2400)
        {
            foreach (Signal sig in signalsToMeasure)
            {
                if (sig.HasSampleRate)
                {
                    sig.SampleRate = sampleRate; //Hz
                }
                measDevice.AssignSignal(sig, out daqPrepareProblems);
            }
            daqSession.AddSignals(measDevice, signalsToMeasure);
            daqSession.PrepareDaq();
            daqSession.StartDaq(DataAcquisitionMode.Unsynchronized);
            dataFetchTimer = new System.Timers.Timer(fetchPeriod); //starts every fetchPeriod ms
            dataFetchTimer.AutoReset = true;
            dataFetchTimer.Elapsed += (o, s) =>
            {
                dataFetchTimer.Stop();
                if (daqSession.IsRunning)
                {
                    daqSession.FillMeasurementValues();
                    foreach (Signal sig in signalsToMeasure)
                    {
                        if (sig.ContinuousMeasurementValues.BufferOverrunOccurred) warningEvent?.Invoke(typeof(DAQ), String.Format("{0} {1} buffer overrun", measDevice, sig));
                        saveDataMethod(sig);
                    }
                }
                dataFetchTimer.Start();
            };
            dataFetchTimer.Start();
        }

        public async Task StartAsync (int fetchPeriod = 500, decimal sampleRate = 2400)
        {
           await Task.Run(() => Start(fetchPeriod, sampleRate));
        }

        public void Stop()
        {
            dataFetchTimer.Stop();
            dataFetchTimer.Dispose();
            daqSession.StopDaq();
        }
    }
}
