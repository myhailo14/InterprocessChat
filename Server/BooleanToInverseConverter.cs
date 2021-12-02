using System;
using System.Windows.Data;

namespace SocketChat
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanToInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            if (value == null || value.GetType() != typeof(bool))
            {
                throw new ArgumentException("The value must be boolean.", nameof(value));
            }

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}