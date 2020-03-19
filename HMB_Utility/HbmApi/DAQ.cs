using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hbm.Api.Common;
using Hbm.Api.Common.Entities;
using Hbm.Api.Common.Entities.Problems;
using Hbm.Api.Common.Entities.Signals;
using Hbm.Api.Common.Enums;

namespace HMB_Utility
{
    public class DAQ
    {
        Device measDevice = null;
        Action<Signal> saveDataMethod;
        public event EventHandler<ProtocolEventArg> eventToProtocol;
        System.Timers.Timer dataFetchTimer = null;
        DaqMeasurement daqSession = null;
        List<Problem> daqPrepareProblems = null;
        List<FoundSignal> signalsToMeasure = null;

        public FoundDevice MeasDevice { get; }
      


        public DAQ(FoundDevice dev, Action<Signal> saveMethod)
        {
            daqPrepareProblems = new List<Problem>();
            daqSession = new DaqMeasurement();
            signalsToMeasure = dev.SignalsToMeasure.ToList();
            saveDataMethod = saveMethod;
            MeasDevice = dev;
            measDevice = MeasDevice.HbmDevice;
        }

        public bool isRunning
        {
            get
            {
                return daqSession.IsRunning;
            }
        }

        ~DAQ()
        {
            daqSession.Dispose();
        }


        private void Start()
        {
           
            int fetchPeriod = AppSettings.FetchPeriod;
            decimal sampleRate = AppSettings.SampleRate;
            try
            {
                foreach (FoundSignal sig in signalsToMeasure)
                {
                    if (sig.HbmSignal.IsMeasurable && sig.HbmSignal.HasSampleRate)
                    {
                        sig.HbmSignal.SampleRate = sampleRate; //Hz
                    }
                    measDevice.AssignSignal(sig.HbmSignal, out daqPrepareProblems);
                    daqSession.AddSignals(measDevice, sig.HbmSignal);
                    eventToProtocol?.Invoke(this, new ProtocolEventArg(string.Format(ProtocolMessage.signalAddedToDaq, sig.Name)));

                }
                daqSession.PrepareDaq();
                daqSession.StartDaq(DataAcquisitionMode.Unsynchronized);
                eventToProtocol?.Invoke(this, new ProtocolEventArg(ProtocolMessage.daqStarted));
                dataFetchTimer = new System.Timers.Timer(fetchPeriod); //starts every fetchPeriod ms
                dataFetchTimer.AutoReset = true;
                dataFetchTimer.Enabled = true;
                dataFetchTimer.Elapsed += (o, s) =>
                {
                    dataFetchTimer.Stop();
                    if (daqSession.IsRunning)
                    {
                        daqSession.FillMeasurementValues();
                        foreach (FoundSignal sig in signalsToMeasure)
                        {
                            if (sig.HbmSignal.ContinuousMeasurementValues.BufferOverrunOccurred) eventToProtocol?.Invoke(this, new ProtocolEventArg(string.Format(ProtocolMessage.signalBufferOverrun, sig.Name, measDevice.Name)));
                            saveDataMethod(sig.HbmSignal);
                        }
                    }
                    dataFetchTimer.Start();
                };
                dataFetchTimer.Start();
            }
            catch (Exception ex)
            {
                eventToProtocol?.Invoke(this, new ProtocolEventArg(ex));
            }
            
        }

        public async Task StartAsync ()
        {
            if (!daqSession.IsRunning)
                await Task.Run(() => Start());
            else eventToProtocol?.Invoke(this, new ProtocolEventArg(ProtocolMessage.daqAlreadyRunning));
        }

        public void Stop()
        {
            try
            {
                dataFetchTimer.Stop();
                dataFetchTimer.Dispose();
                if (daqSession.IsRunning)
                    daqSession.StopDaq();
                eventToProtocol?.Invoke(this, new ProtocolEventArg(ProtocolMessage.daqStopped));
            }
            catch (Exception ex)
            {
                eventToProtocol?.Invoke(this, new ProtocolEventArg(ex));
            }
            
        }
    }
}
