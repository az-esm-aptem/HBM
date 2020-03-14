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
        public static event EventHandler<ProtocolEventArg> eventToProtocol;


        private static bool SaveDevices(List<FoundDevice> devices)
        {
            eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(String.Format(ProtocolMessage.dbPreparation)));
            bool result = false;
            using (HBMContext db = new HBMContext())
            {
                DeviceModel oldDM;
                DeviceModel newDM;
                SignalModel oldSM;
                List<DeviceModel> devicesToAdd = new List<DeviceModel>();
                List<SignalModel> signalsToAdd = new List<SignalModel>();
                foreach (var dev in devices)
                {
                    oldDM = db.Devices.FirstOrDefault(d => d.IpAddress == dev.IpAddress);  //checking if the device already exist in the DB
                    if (oldDM == null)
                    {
                        newDM = new DeviceModel { Name = dev.Name, IpAddress = dev.IpAddress, Model = dev.Model, SerialNo = dev.SerialNo };
                        devicesToAdd.Add(newDM);
                        eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(String.Format(ProtocolMessage.deviceWillBeAdded, dev.Name)));
                        foreach (FoundSignal sig in dev.Signals)
                        {
                            signalsToAdd.Add(new SignalModel { Name = sig.HbmSignal.Name, SampleRate = sig.HbmSignal.SampleRate, UniqueId = sig.HbmSignal.GetUniqueID(), Device = newDM });
                            eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(String.Format(ProtocolMessage.signalWillBeAdded, sig.Name)));
                        }
                    }
                    else
                    {
                        eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(String.Format(ProtocolMessage.deviceAlreadyInDb, dev.Name)));
                        foreach (FoundSignal sig in dev.Signals)
                        {
                            oldSM = db.Signals.FirstOrDefault(s=>s.Name == sig.Name);
                            if (oldSM == null)
                            {
                                signalsToAdd.Add(new SignalModel { Name = sig.HbmSignal.Name, SampleRate = sig.HbmSignal.SampleRate, UniqueId = sig.HbmSignal.GetUniqueID(), Device = oldDM });
                                eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(String.Format(ProtocolMessage.signalWillBeAdded, sig.Name)));
                            }
                            else
                            {
                                eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(String.Format(ProtocolMessage.signalAlreadyInDb, sig.Name)));
                            }

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
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(ex));
                    }
                }
            }
            eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(String.Format(ProtocolMessage.dbPreparationComplete)));
            return result;
        }

        public static async Task<bool> SaveDevicesAsync(List<FoundDevice> devices)
        {
           return await Task.Run(() => SaveDevices(devices));
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
                            eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(ex));
                        }
                    }
                }
                else
                {
                    eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(String.Format(ProtocolMessage.noSignalInDb, sig.Name)));
                }
            }
        }




        public static void SaveDAQMeasurments(Signal sig)
        {
            using (HBMContext db = new HBMContext())
            {
                string sigName = sig.Name;
                List<ValuesModel> valuesToAdd = new List<ValuesModel>();
                try
                {
                    var signalInDB = db.Signals.Where(s => s.Name == sigName).FirstOrDefault();
                    int count = sig.ContinuousMeasurementValues.UpdatedValueCount;

                    if (signalInDB != null)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            valuesToAdd.Add(new ValuesModel
                            {
                                dateTime = DateTime.Now,
                                MeasuredValue = sig.ContinuousMeasurementValues.Values[i],
                                TimeStamp = sig.ContinuousMeasurementValues.Timestamps[0] + i * (double)sig.SampleRate,
                                State = (int)sig.ContinuousMeasurementValues.States[0],
                                Signal = signalInDB
                            }); ;
                        }
                        signalInDB.SampleRate = sig.SampleRate;
                    }
                }
                catch (Exception ex)
                {
                    eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(ex));
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
                        eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(ex));
                    }
                }
            }
        }
    }
}
