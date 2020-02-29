using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HMB_Utility
{
    public class FoundDevice : ViewModelBase
    {
        private Device _device;
        private string _name;
        private string _ipAddress;
        private string _model;
        private string _serialNo;
        private List<Signal> _signals;
        private List<Signal> _signalsToMeasure;

        public Device HbmDevice
        {
            get
            {
                return _device;
            }
            set
            {
                _device = value;
                OnPropertyChanged("Device");
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
        public string IpAddress
        {
            get
            {
                return _ipAddress;
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
                return _model;
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
                return _serialNo;
            }
            set
            {
                _serialNo = value;
                OnPropertyChanged("SerialMo");
            }
        }
        public ObservableCollection<Signal> Signals
        {
            get
            {
                _signals = HbmDevice.GetAllSignals();
                return new ObservableCollection<Signal>(_signals);
            }
            set
            {

            }
        }
        public List<Signal> SignalsToMeasure
        {
            get
            {
                return _signalsToMeasure;
            }
            set
            {
                _signalsToMeasure = value;
            }
        }
   
        public FoundDevice(Device dev)
        {
            HbmDevice = dev;
            Name = dev.Name;
            IpAddress = (dev.ConnectionInfo as EthernetConnectionInfo).IpAddress;
            Model = dev.Model;
            SerialNo = dev.SerialNo;
            SignalsToMeasure = new List<Signal>();
        }

        //For testing!!!! TO DELETE
        public FoundDevice()
        {
            Name = "N1";
            IpAddress = "ip1";
            Model = "M1";
        }
        

}

    //public class FoundDevice
    //{
    //    public Device device { get; private set; }
    //    public string Name { get; private set; }
    //    public string IpAddress { get; private set; }
    //    public string Model { get; private set; }
    //    public string SerialNo { get; private set; }
    //    public List<Signal> signals
    //    {
    //        get
    //        {
    //            return device.GetAllSignals();
    //        }
    //        private set { }
    //    }
    //    public List<Signal> signalsToMeas { get; set; }

    //    public FoundDevice(Device dev)
    //    {
    //        device = dev;
    //        Name = dev.Name;
    //        IpAddress = (dev.ConnectionInfo as EthernetConnectionInfo).IpAddress;
    //        Model = dev.Model;
    //        SerialNo = dev.SerialNo;
    //        signals = new List<Signal>();
    //        signalsToMeas = new List<Signal>();
    //    }
    //}
}
