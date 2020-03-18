using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HMB_Utility
{
    public static class AppSettings
    {
        public static event EventHandler<ProtocolEventArg> eventToProtocol;

        private static int searchPeriod;
        public static int SearchPeriod
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["searchPeriod"], out int period) && period > 0)
                {
                    searchPeriod = period;
                }
                else
                {
                    eventToProtocol?.Invoke(typeof(AppSettings), new ProtocolEventArg(ProtocolMessage.invalidSearchPeriod));
                    throw new Exception(ProtocolMessage.invalidSearchPeriod);
                }
                return searchPeriod;
            }

        }
        private static int searchTime;
        public static int SearchTime
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["searchTime"], out int time) && time > 0)
                {
                    searchTime = time;
                }
                else
                {
                    eventToProtocol?.Invoke(typeof(AppSettings), new ProtocolEventArg(ProtocolMessage.invalidSearchTime));
                    throw new Exception(ProtocolMessage.invalidSearchTime);
                }
                return searchTime;
            }
        }

        private static int fetchPeriod;
        public static int FetchPeriod
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["fetchPeriod"], out int period) && period > 0)
                {
                    fetchPeriod = period;
                }
                else
                {
                    eventToProtocol?.Invoke(typeof(AppSettings), new ProtocolEventArg(ProtocolMessage.invalidFetchPeriod));
                    throw new Exception(ProtocolMessage.invalidFetchPeriod);
                }
                return fetchPeriod;
            }
        }

        private static int sampleRate;
        public static int SampleRate
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["sampleRate"], out int rate) && rate > 0)
                {
                    sampleRate = rate;
                }
                else
                {
                    eventToProtocol?.Invoke(typeof(AppSettings), new ProtocolEventArg(ProtocolMessage.invalidSampleRate));
                    throw new Exception(ProtocolMessage.invalidSampleRate);
                }
                return sampleRate;
            }
        }

        private static int pmxPort;
        public static int PmxPort
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["PMXPort"], out int port) && port > 0)
                {
                    pmxPort = port;
                }
                else
                {
                    eventToProtocol?.Invoke(typeof(AppSettings), new ProtocolEventArg(ProtocolMessage.invalidPortNumber));
                    throw new Exception(ProtocolMessage.invalidPortNumber);
                }
                return pmxPort;
            }
        }

        private static int quantumXPort;
        public static int QuantumXPort
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["QuantumXPort"], out int port) && port > 0)
                {
                    quantumXPort = port;
                }
                else
                {
                    eventToProtocol?.Invoke(typeof(AppSettings), new ProtocolEventArg(ProtocolMessage.invalidPortNumber));
                    throw new Exception(ProtocolMessage.invalidPortNumber);
                }
                return quantumXPort;
            }
        }

        private static int mgcPort;
        public static int MgcPort
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["MGCPort"], out int port) && port > 0)
                {
                    mgcPort = port;
                }
                else
                {
                    eventToProtocol?.Invoke(typeof(AppSettings), new ProtocolEventArg(ProtocolMessage.invalidPortNumber));
                    throw new Exception(ProtocolMessage.invalidPortNumber);
                }
                return mgcPort;
            }
        }

        public static string PmxName = "PMX";
        public static string QuantumxName = "QuantumX";
        public static string MgcName = "QuantumX";
        public static string signalTypeAnalogIn = "Analog Input";
        public static string signalTypeDigital = "Digital Signals";
        public static string signalTypeAll = "All Signals";
        public static string signalTypeCanBeMeasByDAQ = "Can be measured by DAQ";
        public static string ipAddressDefault = "0.0.0.0";
        public static double deviceConnectedFontSize = 14;
        public static double deviceDisconnectedFontSize = 12;
        public static FontWeight deviceConnectedFontWeight = FontWeights.Bold;
        public static FontWeight deviceDisconnectedFontWeight = FontWeights.Normal;






    }
}
