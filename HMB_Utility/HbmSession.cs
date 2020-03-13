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
        public static DaqEnvironment daqEnvironment = null; //main object to work 
        public List<Device> deviceList { get; private set; } // devices found by the scan
        public event EventHandler<ProtocolEventArg> eventToProtocol;
        private System.Timers.Timer searchTimer = null; 
        private HbmSession() 
        {
            daqEnvironment = DaqEnvironment.GetInstance();
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
            if (daqEnvironment != null) daqEnvironment.Dispose();
        }

        //period - The time interval between invocations the scan method to waiting devices gathering
        //searchTime - The time interval for searching. If this time is up and no one device found - the method returns False, and device list is empty
        private bool SearchDevices(object obj)  
        {
            eventToProtocol?.Invoke(this, new ProtocolEventArg("Search started"));
            if(!int.TryParse(ConfigurationManager.AppSettings["searchPeriod"], out int period))
            {
                eventToProtocol?.Invoke(this, new ProtocolEventArg("Invalid search period"));
            }
            if (!int.TryParse(ConfigurationManager.AppSettings["searchTime"], out int searchTime))
            {
                eventToProtocol?.Invoke(this, new ProtocolEventArg("Invalid search time"));
            }
            int time = 0;
            try
            {
                searchTimer = new System.Timers.Timer(period);
                searchTimer.AutoReset = true;
                searchTimer.Elapsed += (s, o) =>
                {
                    time += period;
                    searchTimer.Stop();
                    deviceList = daqEnvironment.Scan();
                    searchTimer.Start();
                };
                searchTimer.Start();

                while (time < searchTime) { };

                searchTimer.Stop();
                searchTimer.Dispose();
            }
            catch (Hbm.Api.Scan.Entities.ScanFailedException ex)
            {
                eventToProtocol?.Invoke(this, new ProtocolEventArg(ex));
            }

            eventToProtocol?.Invoke(this, new ProtocolEventArg("Search ended"));

            if (deviceList.Count > 0)
            {
                deviceList.OrderBy(d => (d.ConnectionInfo as EthernetConnectionInfo).IpAddress);
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
            bool result = false;
            List<Problem> connectToFoundDevicesProblemList = new List<Problem>();
            List<Device> devToConnect = new List<Device>();
            foreach (var dev in devices)
            {
                devToConnect.Add(dev.HbmDevice);
            }
            try
            {
                daqEnvironment.Connect(devToConnect, out connectToFoundDevicesProblemList);
            }
            catch (Exception ex)
            {
                eventToProtocol?.Invoke(this, new ProtocolEventArg(ex));
            }
            
            if (connectToFoundDevicesProblemList.Count>0)
            {
                eventToProtocol?.Invoke(this, new ProtocolEventArg(connectToFoundDevicesProblemList));
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
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
                    eventToProtocol?.Invoke(this, new ProtocolEventArg("Wrong family"));
                    break;
            }
        }

        public void DisconnectDevice(FoundDevice dev)
        {
            try
            {
                daqEnvironment.Disconnect(dev.HbmDevice);
            }
            catch (Exception ex)
            {
                eventToProtocol?.Invoke(this, new ProtocolEventArg(ex));
            }
            
        }
    }
}
