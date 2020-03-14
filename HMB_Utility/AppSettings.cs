using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        




}
}
