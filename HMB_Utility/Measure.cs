using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class Measure
    {
        public static event EventHandler<Exception> exceptionEvent;
        static public List<MeasurementValue> GetMeasurmentValue(List<Device> devices)
        {
            List<MeasurementValue> measurementValues = new List<MeasurementValue>();
            try
            {
                foreach (Device dev in devices)
                {
                    foreach (Connector con in dev.Connectors)
                    {
                        foreach (Channel ch in con.Channels)
                        {
                            foreach (Signal sig in ch.Signals)
                            {
                                if (sig.IsMeasurable)
                                {
                                    measurementValues.Add(sig.GetSingleMeasurementValue());
                                }
                                else measurementValues.Add(new MeasurementValue(0, 0, MeasurementValueState.Overflow));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionEvent(typeof(Measure), ex);
                measurementValues.Clear();
                return measurementValues;
            }
            return measurementValues;
        } 
    }
}
