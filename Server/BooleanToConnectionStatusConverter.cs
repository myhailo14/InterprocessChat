using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SocketChat
{
    public class BooleanToConnectionStatusConverter : IMultiValueConverter
    {
        private const string _serverActive = "Server is active";
        private const string _serverStopped = "Server is stopped";
        private const string _clientConnected = "Connected to server";
        private const string _clientDisconnected = "Disconnected from server";
        private const string _error = "Cannot detect connection status";

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.OfType<bool>().Count() < 1)
            {
                throw new ArgumentException("Invalid array.", nameof(values));
            }
            bool isServer = (bool)values[0];
            bool isActive = (bool)values[1];

            if (isServer && isActive)
            {
                return _serverActive;
            }

            if (isServer && !isActive)
            {
                return _serverStopped;
            }

            if (!isServer && isActive)
            {
                return _clientConnected;
            }

            if (!isServer && !isActive)
            {
                return _clientDisconnected;
            }

            return _error;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}