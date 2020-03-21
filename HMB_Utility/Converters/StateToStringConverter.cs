using System;
using System.Globalization;
using System.Windows.Data;

namespace HMB_Utility
{
    class StateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string state;
            if ((int)value == 0) state = "Valid";
            else state = "Overflow";
            return state;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
