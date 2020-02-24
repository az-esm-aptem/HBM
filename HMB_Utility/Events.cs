using System;
using System.Collections.Generic;
using Hbm.Api.Common.Entities.Problems;

namespace HMB_Utility
{
    public static class Events
    {
        public static event EventHandler<Exception> exceptionEvent;
        public static event EventHandler<List<Problem>> problemEvent;
        public static event EventHandler<string> errorEvent;
    }
}
