using Hbm.Api.Common.Entities.Signals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMB_Utility
{
    public class SignalType
    {
        public SignalType(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        private Type Type { get; set; }
        
    }
}
