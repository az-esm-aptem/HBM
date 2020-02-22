using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
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
   
    
    public class HBM_Object
    {
        private static HBM_Object instance;

        public static DaqEnvironment _daqEnvironment = null; //main object to work 
        public static DaqMeasurement _daqMeasurement = null; //main object to measurment
        Device _device;  //device to connect by IP
        List<Signal> _signalsToMeasure; //list of signals to continuous measurment

        public List<Device> deviceList { get; private set; } // devices found by the scan
        public event EventHandler<Exception> exceptionEvent;
        public event EventHandler<List<Problem>> problemEvent;
        public event EventHandler<string> errorEvent;

        private HBM_Object() 
        {
            _daqEnvironment = DaqEnvironment.GetInstance();
            _daqMeasurement = new DaqMeasurement();
        }

        public static HBM_Object GetInstance()
        {
            if (instance == null)
            {
                instance = new HBM_Object();
            }
            return instance;
            
        }

        ~HBM_Object()
        {
            if (_daqEnvironment != null) _daqEnvironment.Dispose();
            if (_daqMeasurement != null) _daqMeasurement.Dispose();
        }

        //period - The time interval between invocations the scan method to waiting devices gathering
        //searchTime - The time interval for searching. If this time is up and no one device found - returns method returns False, and device list is empty
        public bool SearchDevices(int period = 3000, int searchTime = 30000)  
        {
            int count = 0;
            int foundDevices = 0;
            bool searchEnd = false;
            System.Threading.TimerCallback scanning = (object o) => {
                Console.WriteLine("Invoke {0} {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId.ToString());  //TO DELETE!!!!
                deviceList = _daqEnvironment.Scan();
                if (deviceList.Count == 0)
                {
                    count++;
                }
                else
                    if (deviceList.Count > foundDevices) foundDevices = deviceList.Count;
                    else searchEnd = true;
            };
            System.Threading.Timer searchTimer = new System.Threading.Timer(scanning, null, System.Threading.Timeout.Infinite, 0);

            try
            {
                
                searchTimer.Change(0, period);

                //waiting for found devices
                while (!searchEnd && (period*count) < searchTime) {  };

                if (searchEnd)
                {
                    deviceList = deviceList.OrderBy(d => d.Name).ToList();
                    searchTimer.Dispose();
                    return true;
                }

                if (period * count > searchTime)
                {
                    searchTimer.Dispose();
                    return false;
                }
            }
            catch (Hbm.Api.Scan.Entities.ScanFailedException ex)
            {
                exceptionEvent(this, ex);
            }
            searchTimer.Dispose();
            return false;
        }
        
        public bool ConnectToFoundDevices(List<Device> devList)
        {
            List<Problem> connectToFoundDevicesProblemList = new List<Problem>();
            if (_daqEnvironment.Connect(devList, out connectToFoundDevicesProblemList))
            {
                return true;
            }
            else
            {
                problemEvent(this, connectToFoundDevicesProblemList);
                return false;
            }
            
        }

        public bool ConnectToDeviceByIP(HBMFamily.family family, string ip, int port = -1) 
        {
            List<Problem> ConnectToDeviceByIPProblemList = new List<Problem>();
            int.TryParse(ConfigurationManager.AppSettings["PMXPort"], out int PMXPort);
            int.TryParse(ConfigurationManager.AppSettings["QuantumXPort"], out int QuantumXPort);
            int.TryParse(ConfigurationManager.AppSettings["MGCPort"], out int MGCPort);

            switch (family)
            {
                case (HBMFamily.family.PMX):
                    if (port <= 0)
                        _device = new PmxDevice(ip, PMXPort);
                    else
                        _device = new PmxDevice(ip, port);
                    break;
                case (HBMFamily.family.QuantumX):
                    if (port <= 0)
                        _device = new QuantumXDevice(ip, QuantumXPort);
                    else
                        _device = new QuantumXDevice(ip, port);
                    break;
                case (HBMFamily.family.MGC):
                    if (port <= 0)
                        _device = new MgcDevice(ip, MGCPort);
                    else
                        _device = new MgcDevice(ip, port);
                    break;
                default:
                    errorEvent(this, "Wrong family");
                    return false;
            }

            if (_daqEnvironment.Connect(_device, out ConnectToDeviceByIPProblemList))
            {
                return true;
            }
            else
            {
                problemEvent(this, ConnectToDeviceByIPProblemList);
                return false;
            }
        }

        public bool ChangeSignalName(Device dev, string name) //must contains just ONE signal!!!
        {
            List<Signal> sigToChangeName = dev.GetAllSignals();
            List<Problem> prList = new List<Problem>();

            foreach (Signal s in sigToChangeName)
            {
                s.Name = name;
                dev.AssignSignal(s, out prList);
            }
            
            return false; //TODO
            

        }
    }
}
