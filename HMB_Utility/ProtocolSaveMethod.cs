using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMB_Utility
{
    public class ProtocolSaveMethod
    {
        public event EventHandler<ProtocolEventArg> eventToProtocol;

        //private static string writePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\HBM_Protocol";
        private string writePath;
        string writeFullPath;

        public ProtocolSaveMethod()
        {
            writePath = Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\HBM_Protocols").FullName;
            writeFullPath = $"{writePath}\\Protocol.txt";
        }

        

        public async void Save(string message)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(new FileStream(writeFullPath, FileMode.Append, FileAccess.Write)))
                {
                    await streamWriter.WriteLineAsync(message);
                }
            }
            catch (Exception ex)
            {
                eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(ex));
                eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(String.Format(ProtocolMessage.writeProtocolFileError, writePath)));
            }
        }


        public void Created()
        {
            eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(String.Format(ProtocolMessage.protocolFilePath, writePath)));
        }

    }
}
