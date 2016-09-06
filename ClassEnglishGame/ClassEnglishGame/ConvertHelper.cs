using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace ClassEnglishGame
{
    public static class ConvertHelper
    {
        public static BitmapImage ConverByteToImage(byte[] bytes)
        {
            //Convert byte to image
            var byteStream = new MemoryStream(bytes);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = byteStream;
            image.EndInit();

            return image;
        }

        public static byte[] ImageToByte(BitmapImage image)
        {
            //Convert image to by
            var stream = image.StreamSource;
            byte[] buffer = null;

            if (stream != null && stream.Length > 0)
            {
                using (var br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((Int32)stream.Length);
                }
            }

            return buffer;
        }

        public static byte[] ImageToByte(string imagePath)
        {
            return File.ReadAllBytes(imagePath);
        }
    }
}
