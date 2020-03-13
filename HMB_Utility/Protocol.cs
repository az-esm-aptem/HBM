using Hbm.Api.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HMB_Utility
{
    //Singleton
    public class Protocol : ViewModelBase
    {
        public static readonly DependencyProperty TitleProperty;
        public static readonly DependencyProperty PriceProperty;
        private  ObservableCollection<string> messages = new ObservableCollection<string>();
        private static Protocol instance;
        
        private Protocol()
        {
            messageBrokerSubscribe();
            
        }



        public string Mes
        {
            get { return "dftbnby"; }
        }
            


        public static Protocol GetInstance()
        {
            if (instance == null)
            {
                instance = new Protocol();
            }
            return instance;
        }

        public void Add(object obj, ProtocolEventArg arg)
        {
            foreach (var m in arg.Messages)
            {
                messages.Add(m);
            }
        }


        public ObservableCollection<string> Messages
        {
            get
            {
                return messages;
            }
        }

        private  void messageBrokerSubscribe()
        {
            MessageBroker.DeviceConnected += MessageBroker_DeviceConnected;
            MessageBroker.DeviceDisconnected += MessageBroker_DeviceDisconnected;
            MessageBroker.SampleRatesChanged += MessageBroker_SampleRatesChanged;
            MessageBroker.ChannelOverflowStatusChanged += MessageBroker_ChannelOverflowStatusChanged;
            MessageBroker.SignalNameChanged += MessageBroker_SignalNameChanged;
        }

        private void MessageBroker_SignalNameChanged(object sender, SignalsEventArgs e)
        {
            foreach (var i in e.UniqueSignalIDs)
            {
                Add(new object(), new ProtocolEventArg(String.Format("{0} Device: {1} Channel: {2}", e.Reason, e.UniqueDeviceID, i)));
            }
                
        }

        private void MessageBroker_ChannelOverflowStatusChanged(object sender, ChannelsOverflowStatusEventArgs e)
        {
            foreach (var chOvState in e.ChannelsOverflowState)
            {
                Add(new object(), new ProtocolEventArg(String.Format("{0} Device: {1}, Channel: {2} is in owerflow: {3}", e.Reason, e.UniqueDeviceID, chOvState.ChannelUniqueId, chOvState.IsInOverflow)));
            }
            
        }

        private void MessageBroker_SampleRatesChanged(object sender, SampleRatesEventArgs e)
        {
            foreach(var i in e.UniqueSignalIDs)
            {
                Add(new object(), new ProtocolEventArg(String.Format("{0} Channel: {1}", e.Reason, i)));
            }
           
        }

        private void MessageBroker_DeviceDisconnected(object sender, DeviceEventArgs e)
        {
            Add(new object(), new ProtocolEventArg(String.Format("{0} Device: {1}", e.Reason, e.UniqueDeviceID)));
        }

        private  void MessageBroker_DeviceConnected(object sender, DeviceEventArgs e)
        {
            Add(new object(), new ProtocolEventArg(String.Format("{0} Device: {1}", e.Reason, e.UniqueDeviceID)));
        }



       
    }
}
