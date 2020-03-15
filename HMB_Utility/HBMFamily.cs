using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMB_Utility
{
    public class HBMFamily
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        private int port;
        public int Port
        {
            get
            {
                return port;
            }
        }

        public HBMFamily(string name, int port)
        {
            this.name = name;
            this.port = port;
        }

        public enum family
        {
            PMX,
            QuantumX,
            MGC
        };

    }
}
