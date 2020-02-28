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
    public class FoundDevice
    {
        public Device device { get; private set; }
        public string Name { get; private set; }
        public string IpAddress { get; private set; }
        public string Model { get; private set; }
        public string SerialNo { get; private set; }
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

        
    }
}
