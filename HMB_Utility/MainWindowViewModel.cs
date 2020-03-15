using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

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
    public class MainWindowViewModel: ViewModelBase
    {
        private HbmSession session;
        private List<DAQ> daqSessions;
        private UserCommandAsync searchCommand; //search button
        private UserCommandAsync connectCommand; //connect button
        private UserCommand refreshCommand; //refresh single values
        private UserCommand disconnectCommand; //disconnect the device
        private FoundDevice selectedDevice; //selected in UI
        private UserCommandAsync createDBCommand; // create tables in data base
        private UserCommand useFilterCommand; //filtering signals (see .cfg file)
        private UserCommandAsyncVoid startDaqCommand;
        private UserCommand stopDaqCommand;
        private UserCommand addDeviceByIpCommand;
        private Logger Logs { get; set; }
        public Filter SigFilter { get; set; }
        private Protocol eventProtocoling;
        private ProtocolSaveMethod saveMethod;
        public Family AvailableFamily { get; set; }
        public IpAddress IpToConnect { get; set; }
      
        public ObservableCollection<string> MessagesToProtocol
        {
            get
            {
                return eventProtocoling.Messages;
            }
        }


        public ObservableCollection<FoundDevice> AllDevices { get; set; }
        public MainWindowViewModel()
        {
            session = HbmSession.GetInstance();
            daqSessions = new List<DAQ>();
            AllDevices = new ObservableCollection<FoundDevice>();
            SigFilter = new Filter();
            Logs = new Logger();
            saveMethod = new ProtocolSaveMethod();
            eventProtocoling = Protocol.GetInstance(saveMethod.Save);
            AvailableFamily = new Family();
            subscribeProtocol();
            IpToConnect = new IpAddress(AppSettings.ipAddressDefault);
        }

        private void subscribeProtocol()
        {
            saveMethod.eventToProtocol += eventProtocoling.Add;
            saveMethod.Created();
            Logs.eventToProtocol += eventProtocoling.Add;
            Logs.Created();
            session.eventToProtocol += eventProtocoling.Add;
            DataToDB.eventToProtocol += eventProtocoling.Add;
            AppSettings.eventToProtocol += eventProtocoling.Add;
        }


        public FoundDevice SelectedDevice
        {
            get
            {
                return selectedDevice;
            }
            set
            {
                selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }


        //Push the Search button - searching devices and add them to List<FoundDevice>
        public UserCommandAsync SearchCommand
        {
            get
            {
                return searchCommand ?? (searchCommand = new UserCommandAsync(Search));
            }
        }
        
        private  async Task<bool> Search(object obj)
        {
            bool result = await session.SearchAsync(obj);
            FormDeviceList();
            return result;
        }

        private void FormDeviceList()
        {
            bool isEqual;
            foreach (Device dev in session.deviceList)
            {
                isEqual = false;
                foreach (FoundDevice foundDev in AllDevices)
                {
                    if (isEqual = foundDev.Equals(dev))
                    {
                        break;
                    }
                }
                if (!isEqual)
                {
                    AllDevices.Add(new FoundDevice(dev));
                }
            }

        }



        public UserCommandAsync ConnectCommand
        {
            get
            {
                return connectCommand ?? (connectCommand = new UserCommandAsync(connect, (obj => SelectedDevice != null && !SelectedDevice.HbmDevice.IsConnected)));
            }
        }

        private async Task<bool> connect (object obj)
        {
            bool result = await session.ConnectAsync(new List<FoundDevice> { SelectedDevice });
            FormSignalList();
            return result;
        }

        private void FormSignalList()
        {
            SelectedDevice.GetSignals(); //read signal list from device and make ObservableCollection<FoundSignal>
            SelectedDevice.GetSingleSignalVals(); //read and save in FoundSignal the single measuring values
            SelectedDevice.GetSignalChannel(); //find and save in FoundSignal the channel that the signal belongs
            SelectedDevice.GetSignalConnector(); //find and save in FoundSignal the connector that the signal belongs
        }

        public UserCommand RefreshCommand
        {
            get
            {
                return refreshCommand ?? (refreshCommand = new UserCommand(RefreshSingleVals, (obj =>
                {
                    bool result = false;
                    if (SelectedDevice != null)
                        if (SelectedDevice.Signals.Count > 0)
                            result = true;
                    return result;
                }
                )));
            }
        }

        private void RefreshSingleVals(object obj)
        {
            SelectedDevice.GetSingleSignalVals();
        }
        
        public UserCommand DisconnectCommand
        {
            get
            {
                return disconnectCommand ?? (disconnectCommand = new UserCommand(Disconnect, (obj => SelectedDevice != null && SelectedDevice.HbmDevice.IsConnected)));
            }
        }

        private void Disconnect(object obj)
        {
            DAQ daqSession = daqSessions.FirstOrDefault(s => s.MeasDevice == SelectedDevice);
            if (daqSession!=null && daqSession.isRunning)
            {
                StopDaq(null);
            }
            session.DisconnectDevice(SelectedDevice);
            SelectedDevice.Signals.Clear();
        }

        //if the CreateDB button is needed
        public UserCommandAsync CreateDBCommand
        {
            get
            {
                return createDBCommand ?? (createDBCommand = new UserCommandAsync(CreateDB, (obj => SelectedDevice != null && SelectedDevice.HbmDevice.IsConnected)));
            }
        }

        private async Task<bool> CreateDB(object obj)
        {
            bool result = await DataToDB.SaveDevicesAsync(new List<FoundDevice> { SelectedDevice });
            return result;
        }
        
        public UserCommand UseFilterCommand
        {
            get
            {
                return useFilterCommand ?? (useFilterCommand = new UserCommand(obj=>UseSignalsFilter(obj), (obj => SelectedDevice != null && SelectedDevice.HbmDevice.IsConnected)));
            }
        }

        private void UseSignalsFilter(object obj)
        {
            FormSignalList();
            List<FoundSignal> filtered = new List<FoundSignal>();
                foreach (var s in SelectedDevice.Signals)
                {
                    if (!SigFilter.Check(s))
                    {
                        filtered.Add(s);
                    }
                }
                foreach (var s in filtered)
                {
                    SelectedDevice.Signals.Remove(s);
                }
        }


        public UserCommandAsyncVoid StartDaqCommand
        {
            get
            {
                return startDaqCommand ?? (startDaqCommand = new UserCommandAsyncVoid(StartDaq, (obj => SelectedDevice != null && SelectedDevice.HbmDevice.IsConnected && SelectedDevice.SignalsToMeasure.Count>0 && SigFilter.SelectedType.Name == "Can be measured by DAQ")));
            }
        }

        private async Task StartDaq(object obj)
        {
            bool result = await CreateDB(new object());
            DAQ daqSession = daqSessions.FirstOrDefault(s => s.MeasDevice == SelectedDevice);
            if (daqSession == null)
            {
                daqSession = new DAQ(SelectedDevice, DataToDB.SaveDAQMeasurments);
                daqSessions.Add(daqSession);
                daqSession.eventToProtocol += eventProtocoling.Add;
            }
            
            await daqSession.StartAsync();
        }

        public UserCommand StopDaqCommand
        {
            get
            {
                return stopDaqCommand ?? (stopDaqCommand = new UserCommand(StopDaq, (obj => daqSessions.FirstOrDefault(s => s.MeasDevice == SelectedDevice)!=null)));
            }
        }

        private void StopDaq(object obj)
        {
            DAQ daqSession = daqSessions.FirstOrDefault(s => s.MeasDevice == SelectedDevice);
            if (daqSession != null)
            {
                daqSession.Stop();
                daqSession.eventToProtocol -= eventProtocoling.Add;
                daqSessions.Remove(daqSession);
            }
        }

        public UserCommand AddDeviceByIpCommand
        {
            get
            {
                return addDeviceByIpCommand ?? (addDeviceByIpCommand = new UserCommand(AddDeviceByIp, (obj => AvailableFamily.SelectedFamily!=null && IpToConnect.IsValidIp)));
            }
        }

        private void AddDeviceByIp(object obj)
        {
            session.AddDeviceByIP(AvailableFamily.SelectedFamily, IpToConnect.IP);
            FormDeviceList();
        }
    }
}
