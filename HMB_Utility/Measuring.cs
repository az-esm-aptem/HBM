using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
    public static class Measuring
    {
        public static event EventHandler<string> errorEvent;
        public delegate void StoreSingleMeasurmentData(Signal sig);

        private static void GetMeasurmentValue(List<FoundDevice> devices, StoreSingleMeasurmentData storeMethod)
        {
            
            foreach (var dev in devices)
            {
                if (dev.signalsToMeas.Count > 0)
                {
                    dev.device.ReadSingleMeasurementValue(dev.signalsToMeas);
                    foreach (Signal sig in dev.signalsToMeas)
                    {
                        if (sig.IsMeasurable)
                        {
                            storeMethod(sig);
                        }
                    }
                }
                else
                {
                    errorEvent?.Invoke(typeof(Measuring), "No signals to measure!");
                }
            }
        }

        public static async Task GetMeasurmentValueAsync(List<FoundDevice> devices, StoreSingleMeasurmentData storeMethod)
        {
            await Task.Run(() => GetMeasurmentValue(devices, storeMethod));
        }




    }
}
