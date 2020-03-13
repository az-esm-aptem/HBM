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
           
            int fetchPeriod;
            decimal sampleRate;

            if (!int.TryParse(ConfigurationManager.AppSettings["fetchPeriod"], out fetchPeriod))
            {
                eventToProtocol?.Invoke(this, new ProtocolEventArg("Invalid fetch period"));
            }
            
            if(!decimal.TryParse(ConfigurationManager.AppSettings["sampleRate"], out sampleRate))
            {
                eventToProtocol?.Invoke(this, new ProtocolEventArg("Invalid sample rate"));
            }

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
                }
                daqSession.PrepareDaq();
                daqSession.StartDaq(DataAcquisitionMode.Unsynchronized);
                eventToProtocol?.Invoke(this, new ProtocolEventArg("DAQ started"));
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
                            if (sig.HbmSignal.ContinuousMeasurementValues.BufferOverrunOccurred) eventToProtocol?.Invoke(this, new ProtocolEventArg(string.Format("Signal: {0} Buffer overrun", sig.Name)));
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
            else eventToProtocol?.Invoke(this, new ProtocolEventArg("The DAQ session is already running!"));
        }

        public void Stop()
        {
            try
            {
                dataFetchTimer.Stop();
                dataFetchTimer.Dispose();
                if (daqSession.IsRunning)
                    daqSession.StopDaq();
                eventToProtocol?.Invoke(this, new ProtocolEventArg("DAQ stopped"));
            }
            catch (Exception ex)
            {
                eventToProtocol?.Invoke(this, new ProtocolEventArg(ex));
            }
            
        }
    }
}
