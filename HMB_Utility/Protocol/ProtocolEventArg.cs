using Hbm.Api.Common.Entities.Problems;
using System;
using System.Collections.Generic;

namespace HMB_Utility
{
    public class ProtocolEventArg : EventArgs
    {
        private string message;
        public List<string> Messages { get; set; }
        private List<Problem> ploblemList;
        private Exception ex;

        public ProtocolEventArg(object obj)
        {
            Messages = new List<string>();
            if (obj is string)
            {
                message = (obj as string);
                Messages.Add(message);
            }
            else if (obj is List<Problem>)
            {
                ploblemList = (obj as List<Problem>);
                foreach (var pr in ploblemList)
                {
                    Messages.Add(String.Format("{0} {1} {2}", pr.Message, pr.PropertyName, pr.Position));
                }
            }
            else if (obj is Exception)
            {
                ex = (obj as Exception);
                Messages.Add(ex.Message);
            }
        }


    }
    
}
