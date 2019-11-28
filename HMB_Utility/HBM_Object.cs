using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public enum family
    {
        PMX,
        QuantumX,
        MGC
    };

    public enum defaultPorts
    {
        useDefault = -1,
        PMX = 55000,
        QuantumX = 5001,
        MGC = 7
    }

    


    public class HBM_Object
    {
        
        DaqEnvironment _daqEnvironment; //main object to work 
        DaqMeasurement _daqMeasurement; //main object to measurment
        Device _device;  //device to connect by IP
        List<Signal> _signalsToMeasure; //list of signals to continuous measurment

        public List<Device> deviceList { get; private set; } // devices found by the scan
        public event EventHandler<Exception> exceptionEvent;
        public event EventHandler<List<Problem>> problemEvent;
        public event EventHandler<string> errorEvent;


        public HBM_Object()
        {
            _daqEnvironment = DaqEnvironment.GetInstance();
            _daqMeasurement = new DaqMeasurement();
        }

        public bool SearchDevices()
        {
            try
            {
                deviceList = _daqEnvironment.Scan();
                deviceList = deviceList.OrderBy(d => d.Name).ToList();
                return true;
            }
            catch (Hbm.Api.Scan.Entities.ScanFailedException ex)
            {
                exceptionEvent(this, ex);
            }
            return false;
        }
        
        public bool ConnectToFoundDevices(List<Device> devList)
        {
            List<Problem> ConnectToFoundDevicesProblemList = new List<Problem>();
            if (_daqEnvironment.Connect(devList, out ConnectToFoundDevicesProblemList))
            {
                return true;
            }
            else
            {
                problemEvent(this, ConnectToFoundDevicesProblemList);
                return false;
            }
            
        }

        public bool ConnectToDeviceByIP(family _family, string ip, int port = (int)defaultPorts.useDefault)
        {
            List<Problem> ConnectToDeviceByIPProblemList = new List<Problem>();
            
            switch (_family)
            {
                case (family.PMX):
                    if (port == (int)defaultPorts.useDefault)
                        _device = new PmxDevice(ip, (int)defaultPorts.PMX);
                    else
                        _device = new PmxDevice(ip, port);
                    break;
                case (family.QuantumX):
                    if (port == (int)defaultPorts.useDefault)
                        _device = new QuantumXDevice(ip, (int)defaultPorts.QuantumX);
                    else
                        _device = new QuantumXDevice(ip, port);
                    break;
                case (family.MGC):
                    if (port == (int)defaultPorts.useDefault)
                        _device = new MgcDevice(ip, (int)defaultPorts.QuantumX);
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

            return false;
            

        }
    }
}
