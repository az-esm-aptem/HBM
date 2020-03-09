using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;

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
        Action<Signal> saveDataMethod;
        public event EventHandler<string> warningEvent;
        System.Timers.Timer dataFetchTimer = null;
        DaqMeasurement daqSession = null;
        List<Problem> daqPrepareProblems = null;
        List<FoundSignal> signalsToMeasure = null;


        public DAQ(FoundDevice dev, Action<Signal> saveMethod)
        {
            daqPrepareProblems = new List<Problem>();
            daqSession = new DaqMeasurement();
            signalsToMeasure = dev.SignalsToMeasure.ToList();
            saveDataMethod = saveMethod;
            measDevice = dev.HbmDevice;
        }

        ~DAQ()
        {
            daqSession.Dispose();
        }


        private void Start()
        {
            int fetchPeriod;
            decimal sampleRate;

            int.TryParse(ConfigurationManager.AppSettings["fetchPeriod"], out fetchPeriod);
            decimal.TryParse(ConfigurationManager.AppSettings["sampleRate"], out sampleRate);
            foreach (FoundSignal sig in signalsToMeasure)
            {
                if (sig.HbmSignal.HasSampleRate)
                {
                    sig.HbmSignal.SampleRate = sampleRate; //Hz
                }
                measDevice.AssignSignal(sig.HbmSignal, out daqPrepareProblems);
                daqSession.AddSignals(measDevice, sig.HbmSignal);
            }
            
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
                    foreach (FoundSignal sig in signalsToMeasure)
                    {
                        if (sig.HbmSignal.ContinuousMeasurementValues.BufferOverrunOccurred) warningEvent?.Invoke(typeof(DAQ), String.Format("{0} {1} buffer overrun", measDevice.Name, sig.Name));
                        saveDataMethod(sig.HbmSignal);
                    }
                }
                dataFetchTimer.Start();
            };
            dataFetchTimer.Start();
        }

        public async Task StartAsync ()
        {
           await Task.Run(() => Start());
        }

        public void Stop()
        {
            dataFetchTimer.Stop();
            dataFetchTimer.Dispose();
            daqSession.StopDaq();
        }
    }
}
