using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMB_Utility
{
    public static class ProtocolMessage
    {
        public static string searchStart = "Search started";
        public static string searchEnd = "Search ended";
        public static string invalidSearchPeriod = "Invalid search period";
        public static string invalidSearchTime = "Invalid search time";
        public static string wrongFamily = "Wrong family";
        public static string invalidFetchPeriod = "Invalid fetch period";
        public static string invalidSampleRate = "Invalid sample rate";
        public static string daqStarted = "DAQ session started";
        public static string daqAlreadyRunning = "DAQ session already running";
        public static string signalBufferOverrun = "Signal: {0} Buffer overrun. Device: {1}";
        public static string daqStopped = "DAQ session stopped";
        public static string noSignalInDb = "Signal {0} did not found in the database";
        public static string deviceWillBeAdded = "Device {0} will be added to the database";
        public static string signalWillBeAdded = "Signal {0} will be added to the database";
        public static string deviceAlreadyInDb = "Device {0} already in the database";
        public static string signalAlreadyInDb = "Signal {0} already in the database";
        public static string dbPreparingComplete = "The database preparing comlete";
        public static string logPath = "Log file path: {0}";


    }
}
