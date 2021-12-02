using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SocketChat.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        private readonly Brush _okBrush = new SolidColorBrush(Color.FromArgb(255, 192, 255, 192));
        private readonly Brush _errorBrush = new SolidColorBrush(Color.FromArgb(255, 255, 192, 192));

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