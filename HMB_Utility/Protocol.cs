using Hbm.Api.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        private ObservableCollection<string> messages;
        private static Protocol instance;
        private HbmMessages hbmMessages;
        
        private Action<string> saveProtocolMethod;



        private Protocol(Action<string> saveProtocolMethod)
        {
            this.saveProtocolMethod = saveProtocolMethod;
            messages = new ObservableCollection<string>();
            hbmMessages = new HbmMessages(this.Add);
            BindingOperations.EnableCollectionSynchronization(Messages, new object());
        }


        public static Protocol GetInstance(Action<string> saveProtocolMethod)
        {
            if (instance == null)
            {
                instance = new Protocol(saveProtocolMethod);
            }
            return instance;
        }

        private string AddDate(string s)
        {
            return $"{DateTime.Now}   {s}";
        }

        public void Add(object obj, ProtocolEventArg arg)
        {
            string msgWithDate;
            foreach (string m in arg.Messages)
            {
                msgWithDate = AddDate(m);
                messages.Add(msgWithDate);
                saveProtocolMethod(msgWithDate);
            }
        }


        public ObservableCollection<string> Messages
        {
            get
            {
                return messages;
            }
        }

        

       


    }
}
