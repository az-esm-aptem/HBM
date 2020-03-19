using System;
using System.Globalization;
using System.Windows.Data;

namespace HMB_Utility
{
    class IsConnectedToFontSizeConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? AppSettings.deviceConnectedFontSize : AppSettings.deviceDisconnectedFontSize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
