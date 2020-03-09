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
    public class FoundSignal : ViewModelBase
    {
        private Signal _signal;
        private Connector _connector;
        private Channel _channel;
        private double _value;
        private double _timeStamp;
        private int _state; //0 - valid,  1 - overflow

        public FoundSignal(Signal sig)
        {
            _signal = sig;
        }

        private FoundSignal() { }

        public Signal HbmSignal => _signal;
        public Connector HbmConnector
        {
            get
            {
                return _connector;
            }
            set
            {
                _connector = value;
            }
        }
        public Channel HbmChannel
        {
            get
            {
                return _channel;
            }
            set
            {
                _channel = value;
            }
        }
        public double SingleValue => _value;
        public double SingleTimeStamp => _timeStamp;
        
        public double SingleState => _state;
        public string Name
        {
            get
            {
                return _signal.Name;
            }
        }


        public void GetSingleVals()
        {
            MeasurementValue measVal = _signal.GetSingleMeasurementValue();
            _value = measVal.Value;
            _timeStamp = measVal.Timestamp;
            _state = (int)measVal.State;
        }
        
    }
}
