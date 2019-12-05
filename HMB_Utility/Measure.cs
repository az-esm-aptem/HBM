﻿using System;
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
    public class Measure
    {
       
        public delegate bool fetchDataFromDaq(List <Device> devices); //delegate to the method that describe how to save the data from DAQ
        public static event EventHandler<List<Problem>> problemEvent;
        public static event EventHandler<Exception> exceptionEvent;
        public static System.Threading.Timer dataFetchTimer = null;  //timer for periodically data fetch
       
        static public List<MeasurementValue> GetMeasurmentValue(List<Device> devices)
        {
            List<MeasurementValue> measurementValues = new List<MeasurementValue>();
            try
            {
                foreach (Device dev in devices)
                {
                    foreach (Connector con in dev.Connectors)
                    {
                        foreach (Channel ch in con.Channels)
                        {
                            foreach (Signal sig in ch.Signals)
                            {
                                if (sig.IsMeasurable)
                                {
                                    measurementValues.Add(sig.GetSingleMeasurementValue());
                                }
                                else measurementValues.Add(new MeasurementValue(0, 0, MeasurementValueState.Overflow));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionEvent(typeof(Measure), ex);
                measurementValues.Clear();
                return measurementValues;
            }
            return measurementValues;
        } 

        //Device list contains the Signal list wherein all signals should be measured bu DAQ session
        public static bool DaqPrepare(DaqMeasurement daqMeasurement, List<Device> devices, decimal sampleRate = 2400)  
        {
            List<Problem> daqPrepareProblems = new List<Problem>();
            foreach (Device dev in devices)
            {
                foreach (Connector con in dev.Connectors)
                {
                    foreach (Channel ch in con.Channels)
                    {
                        foreach (Signal sig in ch.Signals)
                        {
                            sig.SampleRate = sampleRate; //Hz
                            dev.AssignSignal(sig, out daqPrepareProblems);
                            daqMeasurement.AddSignals(dev, sig);
                        }
                    }
                }
            }
            if (!daqPrepareProblems.IsEmpty())
            {
                problemEvent(typeof(Measure), daqPrepareProblems);
                if (daqPrepareProblems.HasError()) return false;
            }
            daqMeasurement.PrepareDaq();
            return true;
        }

        public static void DaqRun(DaqMeasurement daqMeasurement, List<Device> devices, fetchDataFromDaq fetchDataMethod, int fetchPeriod = 1000)
        {
            daqMeasurement.StartDaq(DataAcquisitionMode.Auto);
            dataFetchTimer = new Timer(((object obj) => FetchData(daqMeasurement, devices, fetchDataMethod)), null, 0, fetchPeriod); 
            
        }

        private static void FetchData(DaqMeasurement daqMeasurement, List<Device> devices, fetchDataFromDaq fetchDataMethod)
        {
            if (!daqMeasurement.IsRunning)
            {
                return;
            }
            daqMeasurement.FillMeasurementValues();
            fetchDataMethod(devices);
            
        }
    }
}
