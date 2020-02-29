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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HMB_Utility
{
    public class FoundDevice : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Device _device;
        private string _name;
        private string _ipAddress;
        private string _model;
        private string _serialNo;
        public Device device
        {
            get
            {
                return _device;
            }
            private set
            {
                _device = value;
                OnPropertyChanged("device");
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            private set
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
            private set
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
            private set
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
            private set
            {
                _serialNo = value;
                OnPropertyChanged("SerialMo");
            }
        }
        public List<Signal> signals
        {
            get
            {
                return device.GetAllSignals();
            }
            private set { }
        }
        public List<Signal> signalsToMeas { get; set; }

        public FoundDevice(Device dev)
        {
            device = dev;
            Name = dev.Name;
            IpAddress = (dev.ConnectionInfo as EthernetConnectionInfo).IpAddress;
            Model = dev.Model;
            SerialNo = dev.SerialNo;
            signals = new List<Signal>();
            signalsToMeas = new List<Signal>();
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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
