using Hbm.Api.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace HMB_Utility
{
    //Singleton
    public class Protocol
    {
        private ObservableCollection<string> messages = new ObservableCollection<string>();
        private static Protocol instance;
        private HbmMessages hbmMessages;
        private string msgWithDate;

        private Protocol()
        {
            hbmMessages = new HbmMessages(this.Add);
            BindingOperations.EnableCollectionSynchronization(Messages, new object());
        }


        public static Protocol GetInstance()
        {
            if (instance == null)
            {
                instance = new Protocol();
            }
            return instance;
        }

        private void AddDate(string s)
        {
            msgWithDate = $"{DateTime.Now}   {s}";
        }

        public void Add(object obj, ProtocolEventArg arg)
        {
            foreach (var m in arg.Messages)
            {
                AddDate(m);
                messages.Add(msgWithDate);
            }
        }


        public ObservableCollection<string> Messages
        {
            get
            {
                return messages;
            }
        }

        public async void SaveToFile()
        {
            
        }



    }
}
