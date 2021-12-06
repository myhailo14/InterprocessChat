using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SocketChat.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        private readonly Brush _okBrush = new SolidColorBrush(Color.FromArgb(255, 73, 123, 72));
        private readonly Brush _errorBrush = new SolidColorBrush(Color.FromArgb(255, 254, 65, 55));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() != typeof(bool))
            {
                throw new ArgumentException("The value must be bool.");
            }

            bool b = (bool)value;
            return b ? _okBrush : _errorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Brush)value == _okBrush;
        }
    }
}