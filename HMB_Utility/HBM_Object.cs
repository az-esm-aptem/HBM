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


namespace HMB_Utility
{
    class HBM_Object
    {
        DaqEnvironment _daqEnvironment; //main object to work 
        DaqMeasurement _daqMeasurement; //main object to measurment
        Device _device;  //device to use
        List<Signal> _signalsToMeasure; //list of signals to continuous measurment

        public List<Device> _deviceList { get; private set; } // devices found by the scan
        public event EventHandler<Exception> exceptionEvent;
        public event EventHandler<List<Problem>> problemEvent;


        public HBM_Object()
        {
            _daqEnvironment = DaqEnvironment.GetInstance();
            _daqMeasurement = new DaqMeasurement();
        }

        public bool Search()
        {
            try
            {
                _deviceList = _daqEnvironment.Scan();
            }
            catch (Hbm.Api.Scan.Entities.ScanFailedException ex)
            {
                exceptionEvent(this, ex);
            }
            _deviceList = _deviceList.OrderBy(d => d.Name).ToList();
            return true;
        }
        
        public bool ConnectToFoundDevices(List<Device> devList)
        {
            List<Problem> problemList = new List<Problem>();
            _daqEnvironment.Connect(devList, out problemList);
            if (problemList.Count > 0)
            {
                problemEvent(this, problemList);
                return false;
            }
            return true;
        }


    }
}
