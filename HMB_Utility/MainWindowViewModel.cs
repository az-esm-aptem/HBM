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



        public ObservableCollection<FoundDevice> AllDevices { get; set; }
        public MainWindowViewModel()
        {
            session = HbmSession.GetInstance();
            daqSessions = new List<DAQ>();
            AllDevices = new ObservableCollection<FoundDevice>();
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


        //Push the Search button - searching devices and add it to List<FoundDevice>
        public UserCommandAsync SearchCommand
        {
            get
            {
                return searchCommand ?? (searchCommand = new UserCommandAsync(FormDeviceList));
            }
        }
        
        private  async Task<bool> FormDeviceList(object obj)
        {
            bool result = await session.SearchAsync(obj);
            AllDevices.Clear();
            foreach (var dev in session.deviceList)
            {
                AllDevices.Add(new FoundDevice(dev));
            }
            return result;
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
            session.DisconnectDevice(SelectedDevice);
            SelectedDevice.Signals.Clear();
        }


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
                return useFilterCommand ?? (useFilterCommand = new UserCommand(obj=>UseSignalsFilter(obj)));
            }
        }

        private void UseSignalsFilter(object obj)
        {
            bool isChecked = (bool)obj;
            List<FoundSignal> filtered = new List<FoundSignal>();
            if (isChecked)
            {
                foreach (var s in SelectedDevice.Signals)
                {
                    if (!TypeFilter.Check(s.HbmSignal))
                    {
                        filtered.Add(s);
                    }
                }
                foreach (var s in filtered)
                {
                    SelectedDevice.Signals.Remove(s);
                }
            }
            else
            {
                FormSignalList();
            }
        }








    }
}
