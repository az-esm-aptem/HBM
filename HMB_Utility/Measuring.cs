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
        public static void GetMeasurmentValue(List<Device> devices, StoreSingleMeasurmentData storeMethod)
        {
            List<MeasurementValue> measurementValues = new List<MeasurementValue>();
            foreach (Device dev in devices)
            {
                dev.ReadSingleMeasurementValueOfAllSignals();
                List<Signal> AllSignals = dev.GetAllSignals();
                foreach (Signal sig in AllSignals)
                {
                    if (TypeFilter.Check(sig) && sig.IsMeasurable) //checking the signal type
                    {
                        storeMethod(sig);
                        measurementValues.Add(sig.GetSingleMeasurementValue());
                    }
                }
            }
        } 

        


    }
}
