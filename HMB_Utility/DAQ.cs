using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
        System.Threading.Timer dataFetchTimer = null;
        DaqMeasurement daqSession = null;
        List<Problem> daqPrepareProblems = null;
        List<Signal> signalsToMeasure = null;


        public DAQ(Device dev, SaveDataFromDaq saveMethod)
        {
            daqPrepareProblems = new List<Problem>();
            daqSession = new DaqMeasurement();
            signalsToMeasure = new List<Signal>();
            dataFetchTimer = new System.Threading.Timer(FetchData, null, Timeout.Infinite, 0);
            saveDataMethod = saveMethod;
            measDevice = dev;
        }
        public void Start(int fetchPeriod = 500, decimal sampleRate = 2400)
        {
            List<Signal> AllSignals = measDevice.GetAllSignals();
            foreach (Signal sig in AllSignals)
            {
                if (TypeFilter.Check(sig)) //checking the signal type
                {
                    if (sig.HasSampleRate)
                    {
                        sig.SampleRate = sampleRate; //Hz
                    }
                    measDevice.AssignSignal(sig, out daqPrepareProblems);
                    signalsToMeasure.Add(sig);
                }
            }
            daqSession.AddSignals(measDevice, signalsToMeasure);
            daqSession.PrepareDaq();
            daqSession.StartDaq(DataAcquisitionMode.Unsynchronized);
            dataFetchTimer.Change(0, fetchPeriod);//starts every fetchPeriod ms

        }

        public void Stop()
        {
            dataFetchTimer.Change(Timeout.Infinite, Timeout.Infinite);
            dataFetchTimer.Dispose();
            daqSession.StopDaq();
            daqSession.Dispose();
        }


        private void FetchData(object o)
        {
            if (daqSession.IsRunning)
            {
                daqSession.FillMeasurementValues();
                foreach (Signal sig in signalsToMeasure)
                {
                    if (sig.ContinuousMeasurementValues.BufferOverrunOccurred) warningEvent?.Invoke(typeof(DAQ), String.Format("{0} {1} buffer overrun", measDevice, sig));
                    saveDataMethod(sig);
                }
            }
        }
    }
}
