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
        private string _name;

        public FoundSignal(Signal sig)
        {
            HbmSignal = sig;
            Name = sig.Name;
        }

        private FoundSignal() { }

        public Signal HbmSignal
        {
            get
            {
                return _signal;
            }
            set
            {
                _signal = value;
            }
        }
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
                OnPropertyChanged("HbmChannel");

            }
        }
        public double SingleValue
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged("SingleValue");

            }
        }

        public double SingleTimeStamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                _timeStamp = value;
                OnPropertyChanged("SingleTimeStamp");

            }
        }

        
        public int SingleState
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                OnPropertyChanged("SingleState");
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");

            }
        }


        public void GetSingleVals()
        {
            MeasurementValue measVal = HbmSignal.GetSingleMeasurementValue();
            SingleValue = measVal.Value;
            SingleTimeStamp = measVal.Timestamp;
            SingleState = (int)measVal.State;
        }
        
    }
}
