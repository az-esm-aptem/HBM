using Hbm.Api.Common.Messaging;
using System;

namespace HMB_Utility
{
    public class HbmMessages
    {
        public delegate void ProtocolMethod(object obj, ProtocolEventArg arg);
        private ProtocolMethod protocolMethod;

        public HbmMessages(ProtocolMethod method)
        {
            protocolMethod = method;
            messageBrokerSubscribe();
        }

        private string connectedMessage = "Device connected ({0})";
        private string disconnectedMessage = "Device disconnected ({0})";
        private string signalNameChangedMessage = "Signal name changed. Device: {0} Signal: {1}";
        private string channelOverflowStatusChanged = "Channel {0} owerflow status changed. Overflow: {1}. Device {2}";
        private string sampleRateChanged = "Signal sample rate changed. Device: {0} Signal: {1}";


        private void messageBrokerSubscribe()
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
                protocolMethod(new object(), new ProtocolEventArg(String.Format(signalNameChangedMessage, e.UniqueDeviceID, i)));
            }

        }

        private void MessageBroker_ChannelOverflowStatusChanged(object sender, ChannelsOverflowStatusEventArgs e)
        {
            foreach (var chOvState in e.ChannelsOverflowState)
            {
                protocolMethod(new object(), new ProtocolEventArg(String.Format(channelOverflowStatusChanged, chOvState.ChannelUniqueId, chOvState.IsInOverflow, e.UniqueDeviceID)));
            }

        }

        private void MessageBroker_SampleRatesChanged(object sender, SampleRatesEventArgs e)
        {
            foreach (var i in e.UniqueSignalIDs)
            {
                protocolMethod(new object(), new ProtocolEventArg(String.Format(sampleRateChanged, e.UniqueDeviceID, i)));
            }

        }

        private void MessageBroker_DeviceDisconnected(object sender, DeviceEventArgs e)
        {
            protocolMethod(new object(), new ProtocolEventArg(String.Format(disconnectedMessage, e.UniqueDeviceID)));
        }

        private void MessageBroker_DeviceConnected(object sender, DeviceEventArgs e)
        {
            protocolMethod(new object(), new ProtocolEventArg(String.Format(connectedMessage, e.Reason, e.UniqueDeviceID)));
        }
    }
}
