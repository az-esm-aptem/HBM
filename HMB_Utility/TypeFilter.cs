using System.Configuration;
using Hbm.Api.Common.Entities.Signals;


namespace HMB_Utility
{
    public static class TypeFilter
    {
        public static bool Check(Hbm.Api.Common.Entities.Signals.Signal sig)
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
