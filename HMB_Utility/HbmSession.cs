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
   
    
    public class HbmSession
    {
        private static HbmSession instance;
        public static DaqEnvironment _daqEnvironment = null; //main object to work 
        public List<Device> deviceList { get; private set; } // devices found by the scan
        public event EventHandler<Exception> exceptionEvent;
        public event EventHandler<List<Problem>> problemEvent;
        public event EventHandler<string> errorEvent;
        private System.Timers.Timer searchTimer = null; 
        private HbmSession() 
        {
            _daqEnvironment = DaqEnvironment.GetInstance();
        }

        public static HbmSession GetInstance()
        {
            if (instance == null)
            {
                instance = new HbmSession();
            }
            return instance;
            
        }

        ~HbmSession()
        {
            if (_daqEnvironment != null) _daqEnvironment.Dispose();
        }

        //period - The time interval between invocations the scan method to waiting devices gathering
        //searchTime - The time interval for searching. If this time is up and no one device found - the method returns False, and device list is empty
        private bool SearchDevices(object obj)  
        {
            int.TryParse(ConfigurationManager.AppSettings["searchPeriod"], out int period);
            int.TryParse(ConfigurationManager.AppSettings["searchTime"], out int searchTime);
            int time = 0;
            searchTimer = new System.Timers.Timer(period);
            searchTimer.AutoReset = true;
            searchTimer.Elapsed += (s, o) =>
            {
                time+=period;
                searchTimer.Stop();
                try
                {
                    deviceList = _daqEnvironment.Scan();
                }
                catch (Hbm.Api.Scan.Entities.ScanFailedException ex)
                {
                    exceptionEvent?.Invoke(this, ex);
                }
                searchTimer.Start();
            };
            searchTimer.Start();

            while (time < searchTime) { };

            searchTimer.Stop();
            searchTimer.Dispose();

            if (deviceList.Count > 0)
            {
                deviceList.OrderBy(d => d.Name);
                return true;
            }
            else return false;
        }

        public async Task<bool> SearchAsync(object obj)
        {
            return await Task.Run(() => SearchDevices(obj));
        }


        private bool ConnectToDevice(List<FoundDevice> devices)
        {
            List<Problem> connectToFoundDevicesProblemList = new List<Problem>();
            List<Device> devToConnect = new List<Device>();
            foreach (var dev in devices)
            {
                devToConnect.Add(dev.HbmDevice);
            }
            if (_daqEnvironment.Connect(devToConnect, out connectToFoundDevicesProblemList))
            {
                return true;
            }
            else
            {
                problemEvent?.Invoke(this, connectToFoundDevicesProblemList);
                return false;
            }
        }

        public async Task<bool> ConnectAsync(List<FoundDevice> devices)
        {
            return await Task.Run(() => ConnectToDevice(devices));
        }


        public void AddDeviceByIP(HBMFamily.family family, string ip, int port = -1) 
        {
            List<Problem> ConnectToDeviceByIPProblemList = new List<Problem>();
            int.TryParse(ConfigurationManager.AppSettings["PMXPort"], out int PMXPort);
            int.TryParse(ConfigurationManager.AppSettings["QuantumXPort"], out int QuantumXPort);
            int.TryParse(ConfigurationManager.AppSettings["MGCPort"], out int MGCPort);

            switch (family)
            {
                case (HBMFamily.family.PMX):
                    if (port <= 0)
                        deviceList.Add(new PmxDevice(ip, PMXPort));
                    else
                        deviceList.Add(new PmxDevice(ip, port));
                    break;
                case (HBMFamily.family.QuantumX):
                    if (port <= 0)
                        deviceList.Add(new QuantumXDevice(ip, QuantumXPort));
                    else
                        deviceList.Add(new QuantumXDevice(ip, port));
                    break;
                case (HBMFamily.family.MGC):
                    if (port <= 0)
                        deviceList.Add(new MgcDevice(ip, MGCPort));
                    else
                        deviceList.Add(new MgcDevice(ip, port));
                    break;
                default:
                    errorEvent?.Invoke(this, "Wrong family");
                    break;
            }
        }
    }
}
