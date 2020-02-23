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

using DB;

namespace HMB_Utility
{
    
    public static class DataToDB
    {
        public static void SaveDevices(List<Device>devList)
        {
            using (HBMContext db = new HBMContext())
            {
                foreach (Device dev in devList)
                {
                    string ip = (dev.ConnectionInfo as EthernetConnectionInfo).IpAddress;

                    DeviceModel oldDM = db.Devices.FirstOrDefault(d => d.IpAddress == ip);  //checking if the device already exist in the DB
                    if (oldDM != null) continue; //if already exist - continue with next device

                    //adding new device in the DB
                    DeviceModel newDM = new DeviceModel { Name = dev.Name, IpAddress = ip, Model = dev.Model, SerialNo = dev.SerialNo };
                    db.Devices.Add(newDM);
                    db.SaveChanges();
                    
                    List<Signal> signals = dev.GetAllSignals();

                    //adding signals
                    foreach (Signal sig in signals)
                    {
                        if ((sig is AnalogInSignal))
                        {
                            if (sig.IsMeasurable)
                            {
                                db.Signals.Add(new SignalModel { Name = sig.Name, SampleRate = sig.SampleRate, UniqueId = sig.GetUniqueID(), Device = newDM });
                                db.SaveChanges();
                            }
                        }
                        
                    }
                }

            }
                
        }

        public static void SaveSingleMeasurments(Signal sig)
        {
            
            using (HBMContext db = new HBMContext())
            {
                string sigName = sig.Name;
                var signalInDB = db.Signals.Where(s => s.Name == sigName).FirstOrDefault();
                if (signalInDB != null)
                {
                    ValuesModel newVal = new ValuesModel
                    {
                        dateTime = DateTime.Now,
                        MeasuredValue = sig.GetSingleMeasurementValue().Value,
                        TimeStamp = sig.GetSingleMeasurementValue().Timestamp,
                        State = (int)sig.GetSingleMeasurementValue().State,
                        Signal = signalInDB
                    };
                    db.Values.Add(newVal);
                    db.SaveChanges();
                }
                
            }

        }

        
    }
}
