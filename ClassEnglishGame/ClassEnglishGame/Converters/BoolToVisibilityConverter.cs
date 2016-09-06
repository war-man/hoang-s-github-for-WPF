using System.Windows;
using System.Windows.Data;

namespace ClassEnglishGame.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value is bool) && (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() == typeof(Visibility))
            {
                return (Visibility)value != Visibility.Collapsed;
            }

            return false;
        }
    }
}
