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
            deviceList = new List<Device>();
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
            eventToProtocol?.Invoke(this, new ProtocolEventArg(ProtocolMessage.searchStart));

            int time = 0;
            int period = AppSettings.SearchPeriod;
            int searchTime = AppSettings.SearchTime;
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

            eventToProtocol?.Invoke(this, new ProtocolEventArg(ProtocolMessage.searchEnd));

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


        public void AddDeviceByIP(HBMFamily family, string ip) 
        {

            if(family.Name == AppSettings.PmxName)
            {
                deviceList.Add(new PmxDevice(ip, family.Port));
            }
            else if (family.Name == AppSettings.QuantumxName)
            {
                deviceList.Add(new QuantumXDevice(ip, family.Port));
            }
            else if (family.Name == AppSettings.MgcName)
            {
                deviceList.Add(new MgcDevice(ip, family.Port));
            }
            else
            {
                eventToProtocol?.Invoke(this, new ProtocolEventArg(ProtocolMessage.wrongFamily));
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
