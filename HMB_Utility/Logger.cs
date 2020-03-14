using Hbm.Api.Common.Messaging;
using Hbm.Api.Logging;
using Hbm.Api.Logging.Enums;
using Hbm.Api.Logging.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMB_Utility
{
    public class Logger
    {
        private ILogger hbmLogger;
        private LogContext logContextMeasuring;
        private LogContext logContextProblems;
        private int logNumberDummy = 0;
        public event EventHandler<ProtocolEventArg> eventToProtocol;


        public Logger()
        {
            prepareHbmLogger();
            createLogEntries();
        }

        private void prepareHbmLogger()
        {
            LogManager.Start(LoggingFramework.NLog, LogLevel.Error | LogLevel.Warn | LogLevel.Info);
            hbmLogger = LogManager.CreateLogger("Logger");
            logContextMeasuring = new LogContext("Measurement");
            logContextProblems = new LogContext("Problems");
        }

        public void Created()
        {
            eventToProtocol?.Invoke(this, new ProtocolEventArg(string.Format(ProtocolMessage.logPath, LogManager.LogFolder)));
        }

        private void createLogEntries()
        {
            hbmLogger.ErrorFormat(logContextMeasuring, "Error: Log entry number {0} for measurement context", logNumberDummy++);
            hbmLogger.DebugFormat(logContextMeasuring, "Debug: Log entry number {0} for measurement context", logNumberDummy++);
            hbmLogger.InfoFormat(logContextProblems, "Info: Log entry number {0} for problems context", logNumberDummy++);
        }

        public void OpenLogFile()
        {
            try
            {
                // Open the directory in which the logfile will be created
                System.Diagnostics.Process.Start("explorer.exe", LogManager.LogFolder);
                // Use LogManager.CurrentLogFile to get the full path including the filename of the logfile 
                Console.WriteLine("Full path to logfile is: " + LogManager.CurrentLogFile);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problems opening path to logfile: " + ex.Message);
            }
        }

        
    }
}
