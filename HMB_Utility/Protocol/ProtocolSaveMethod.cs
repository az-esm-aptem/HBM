using System;
using System.IO;
using System.Threading;

namespace HMB_Utility
{
    public class ProtocolSaveMethod
    {
        public event EventHandler<ProtocolEventArg> eventToProtocol;

        private string writePath;
        string writeFullPath;
        static Mutex mutexObj = new Mutex();

        public ProtocolSaveMethod()
        {
            writePath = Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\HBM_Protocols").FullName;
            writeFullPath = $"{writePath}\\Protocol.txt";
        }

        

        public async void Save(string message)
        {
            mutexObj.WaitOne();
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
                mutexObj.ReleaseMutex();
            }
            mutexObj.ReleaseMutex();
        }


        public void Created()
        {
            eventToProtocol?.Invoke(typeof(DataToDB), new ProtocolEventArg(String.Format(ProtocolMessage.protocolFilePath, writePath)));
        }

    }
}
