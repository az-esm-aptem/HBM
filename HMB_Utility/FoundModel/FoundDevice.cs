﻿using System.Linq;
using System.Collections.ObjectModel;
using Hbm.Api.Common.Entities;
using Hbm.Api.Common.Entities.ConnectionInfos;

namespace HMB_Utility
{
    public class FoundDevice : ViewModelBase
    {
        private Device _device;
        private ObservableCollection<FoundSignal> _signals;
        private ObservableCollection<FoundSignal> _signalsToMeasure;
        private string _ipAddress;
        private string _model;
        private string _serialNo;
        private string _name;
        private bool _isConnected;

        private FoundDevice() { }

        public FoundDevice(Device dev)
        {
            HbmDevice = dev;
            Signals = new ObservableCollection<FoundSignal>();
            SignalsToMeasure = new ObservableCollection<FoundSignal>();
        }

        public bool IsConnected
        {
            get
            {
                return _isConnected = HbmDevice.IsConnected;
            }
            set
            {
                _isConnected = value;
                OnPropertyChanged("IsConnected");
            }
        }

        

        public Device HbmDevice
        {
            get
            {
                return _device;
            }
            set
            {
                _device = value;
                OnPropertyChanged("HbmDevice");
            }
        }
        public string Name
        {
            get
            {
                return _name = HbmDevice.Name;  
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public string IpAddress
        {
            get
            {
                return _ipAddress = (HbmDevice.ConnectionInfo as EthernetConnectionInfo).IpAddress;
            }
            set
            {
                _ipAddress = value;
                OnPropertyChanged("IpAddress");
            }

        }
        public string Model
        {
            get
            {
                return _model = HbmDevice.FamilyName;
            }
            set
            {
                _model = value;
                OnPropertyChanged("Model");
            }
        }
        public string SerialNo
        {
            get
            {
                return _serialNo = HbmDevice.SerialNo;
            }
            set
            {
                _serialNo = value;
                OnPropertyChanged("SerialNo");

            }
        }
        public ObservableCollection<FoundSignal> Signals
        {
            get
            {
                return _signals;
            }
            set
            {
                _signals = value;
                OnPropertyChanged("Signals");
            }
        }

        public void GetSignals()
        {
            foreach (var s in HbmDevice.GetAllSignals())
            {
                if (Signals.Where(sig=>sig.HbmSignal == s).ToList().Count == 0)
                    Signals.Add(new FoundSignal(s));
            }
            
        }

        public void GetSingleSignalVals()
        {
            HbmDevice.ReadSingleMeasurementValueOfAllSignals();
            foreach (var s in Signals)
            {
                s.GetSingleVals();
            }
        }

        public void GetSignalChannel()
        {
            foreach (var s in Signals)
            {
                s.HbmChannel = HbmDevice.FindChannel(s.HbmSignal);
            }
        }

        public void GetSignalConnector()
        {
            foreach (var s in Signals)
            {
                s.HbmConnector = HbmDevice.FindConnector(s.HbmSignal);
            }
        }


        public ObservableCollection<FoundSignal> SignalsToMeasure
        {
            get
            {
                return _signalsToMeasure;
            }
            set
            {
                _signalsToMeasure = value;
                OnPropertyChanged("SignalsToMeasure");

            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            FoundDevice dev = obj as FoundDevice;
            if (dev == null) return false;

            return ((dev.HbmDevice.ConnectionInfo as EthernetConnectionInfo).IpAddress == (this.HbmDevice.ConnectionInfo as EthernetConnectionInfo).IpAddress);
        }

        public bool Equals(FoundDevice dev)
        {
            if (dev == null) return false;

            return ((dev.HbmDevice.ConnectionInfo as EthernetConnectionInfo).IpAddress == (this.HbmDevice.ConnectionInfo as EthernetConnectionInfo).IpAddress);
        }

        public bool Equals(Device dev)
        {
            if (dev == null) return false;

            return ((dev.ConnectionInfo as EthernetConnectionInfo).IpAddress == (this.HbmDevice.ConnectionInfo as EthernetConnectionInfo).IpAddress);
        }





    }

  
}
