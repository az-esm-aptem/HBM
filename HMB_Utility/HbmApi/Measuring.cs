using System;
using System.Threading.Tasks;
using Hbm.Api.Common.Entities.Signals;

namespace HMB_Utility
{
    public static class Measuring
    {
        public static event EventHandler<string> errorEvent;

        private static bool GetMeasurmentValue(FoundDevice dev, Action<Signal> storeMethod)
        {
            
            bool result = false;
            if (dev.SignalsToMeasure.Count > 0)
            {
                dev.HbmDevice.ReadSingleMeasurementValueOfAllSignals();
                foreach (FoundSignal sig in dev.SignalsToMeasure)
                {
                    if (sig.HbmSignal.IsMeasurable)
                    {
                        storeMethod(sig.HbmSignal);
                    }
                }
                result = true;
            }
            else
            {
                errorEvent?.Invoke(typeof(Measuring), "No signals to measure!");
            }
            return result;
        }

        public static async Task<bool> GetMeasurmentValueAsync(FoundDevice dev, Action<Signal> storeMethod)
        {
            return await Task.Run(() => GetMeasurmentValue(dev, storeMethod));
        }


        

    }
}
