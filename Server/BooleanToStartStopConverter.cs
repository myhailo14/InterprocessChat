using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SocketChat
{
    public class BooleanToStartStopConverter : IMultiValueConverter
    {
        private const string _clientDisconnect = "Disconnect";
        private const string _clientConnect = "Connect";

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 1 && values[0].GetType() != typeof(bool))
            {
                throw new ArgumentException("Invalid values.", nameof(values));
            }

            bool isActive = (bool) values[0];
            return isActive ? _clientDisconnect : _clientConnect;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}