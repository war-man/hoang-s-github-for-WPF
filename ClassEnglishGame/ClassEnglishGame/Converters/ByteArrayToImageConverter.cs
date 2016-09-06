using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ClassEnglishGame.Converters
{
    public class ByteArrayToImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is byte[])
            {
                return ConvertHelper.ConverByteToImage((byte[])value);
            }
            return new BitmapImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : ConvertHelper.ImageToByte((BitmapImage)value);
        }

        #endregion
    }
}