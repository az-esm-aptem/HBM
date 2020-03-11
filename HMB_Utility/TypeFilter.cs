using Hbm.Api.Common.Entities.Signals;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;


namespace HMB_Utility
{
    public static class TypeFilter
    {
        public static bool Check(Signal sig, Filter filter)
        {

            switch (ConfigurationManager.AppSettings["SignalsType"])
            {
                case "AI":
                    return (sig is AnalogInSignal);
                case "DI":
                    return (sig is DigitalSignal);
                case "All":
                    return (sig is Signal);
                default:
                    return (sig is Signal);
            }
        }
    }
}
