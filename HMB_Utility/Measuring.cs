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
        
        public delegate void StoreSingleMeasurmentData(Signal sig);

        public static void GetMeasurmentValue(FoundDevice dev, StoreSingleMeasurmentData storeMethod)
        {
            List<MeasurementValue> measurementValues = new List<MeasurementValue>();
            dev.device.ReadSingleMeasurementValue(dev.signalsToMeas);
            foreach (Signal sig in dev.signalsToMeas)
            {
                if (sig.IsMeasurable)
                {
                    storeMethod(sig);
                    measurementValues.Add(sig.GetSingleMeasurementValue());
                }
            }
        }
        

    }
}
