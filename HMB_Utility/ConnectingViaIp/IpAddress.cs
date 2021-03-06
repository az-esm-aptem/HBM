﻿using System;
using System.ComponentModel;

namespace HMB_Utility
{
    public class IpAddress : IDataErrorInfo
    {
        private string ip;
        private string error;
        public string IP
        {
            get
            {
                return ip;
            }
            set
            {
                ip = value.Replace(',', '.');
            }
        }

        public bool IsValidIp { get; set; }
        public IpAddress(string ip)
        {
            this.ip = ip;
        }
        public string this[string columnName]
        {
            get
            {
                error = String.Empty;
                switch (columnName)
                {
                    case "IP":
                        if (!(IsValidIp = IsValid(ip)))
                        {
                            error = AppSettings.invalidIp;
                        }
                        break;
                }
                return error;
            }
        }

        static bool IsValid(string address)
        {
           
            System.Text.RegularExpressions.Regex IpMatch =
            new System.Text.RegularExpressions.Regex(@"\d{1,3}[.,]\d{1,3}[.,]\d{1,3}[.,]\d{1,3}");
           
            return IpMatch.IsMatch(address);
        }

        public string Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
            }
        }
    }
}
