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
        //private delegate void DeviceEventHandler(DeviceEventArgs e); //event handler
        //MessageBroker _mesBroker;

        public HBM_Object()
        {
            _daqEnvironment = DaqEnvironment.GetInstance();
            _daqMeasurement = new DaqMeasurement();
        }

        public void Search()
        {

        }
    }
}
