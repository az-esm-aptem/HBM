using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace HMB_Utility
{
    public class DeviceInfoConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string result = string.Empty;
            foreach(var str in values)
            {
                if (str != null)
                {
                    result += $"{str}\n";
                }
                else
                {
                    result += $"Unknown\n";
                }
            }
            return result;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
