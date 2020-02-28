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
using System.Threading;

namespace HMB_Utility
{
    
    public static class DataToDB
    {
        public static event EventHandler<Exception> exceptionEvent;
        public static event EventHandler<string> errorEvent;

        private static void SaveDevices(List<FoundDevice> devices)
        {
            using (HBMContext db = new HBMContext())
            {
                DeviceModel oldDM;
                DeviceModel newDM;
                List<DeviceModel> devicesToAdd = new List<DeviceModel>();
                List<SignalModel> signalsToAdd = new List<SignalModel>();
                foreach (var dev in devices)
                {
                    oldDM = db.Devices.FirstOrDefault(d => d.IpAddress == dev.IpAddress);  //checking if the device already exist in the DB
                    if (oldDM == null)
                    {
                        newDM = new DeviceModel { Name = dev.Name, IpAddress = dev.IpAddress, Model = dev.Model, SerialNo = dev.SerialNo };
                        devicesToAdd.Add(newDM);
                        foreach (Signal sig in dev.signals)
                        {
                            signalsToAdd.Add(new SignalModel { Name = sig.Name, SampleRate = sig.SampleRate, UniqueId = sig.GetUniqueID(), Device = newDM });
                        }
                    }
                }
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Devices.AddRange(devicesToAdd);
                        db.Signals.AddRange(signalsToAdd);
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        exceptionEvent?.Invoke(typeof(DataToDB), ex);
                    }
                }
            }
        }

        public static async Task SaveDevicesAsync(List<FoundDevice> devices)
        {
            await Task.Run(() => SaveDevices(devices));
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
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.Values.Add(newVal);
                            db.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            exceptionEvent?.Invoke(typeof(DataToDB), ex);
                        }
                    }
                }
                else
                {
                    errorEvent?.Invoke(typeof(DataToDB), String.Format("No signal {0} found in the data base", sig));
                }
            }
        }




        public static void SaveDAQMeasurments(Signal sig)
        {
            using (HBMContext db = new HBMContext())
            {
                string sigName = sig.Name;
                List<ValuesModel> valuesToAdd = new List<ValuesModel>();
                var signalInDB = db.Signals.Where(s => s.Name == sigName).FirstOrDefault();
                int count = sig.ContinuousMeasurementValues.UpdatedValueCount;

                if (signalInDB != null)
                {
                    for(int i = 0; i<count; i++)
                    {
                        valuesToAdd.Add(new ValuesModel
                        {
                            dateTime = DateTime.Now,
                            MeasuredValue = sig.ContinuousMeasurementValues.Values[i],
                            TimeStamp = sig.ContinuousMeasurementValues.Timestamps[0]+i*(double)sig.SampleRate,
                            State = (int)sig.ContinuousMeasurementValues.States[0],
                            Signal = signalInDB
                        }); ;
                    }
                }
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Values.AddRange(valuesToAdd);
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        exceptionEvent?.Invoke(typeof(DataToDB), ex);
                    }
                }
            }
        }
    }
}
