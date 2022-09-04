using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Drawing;
using System.IO;

namespace WallpaperManager.Controls
{
    [ValueConversion(typeof(String), typeof(ImageSource))]
    internal class StringToImageSourceConverter : IValueConverter
    {
        private const string OrientationQuery = "System.Photo.Orientation";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string valueString))
            {
                return null;
            }
            try
            {
                // ImageSource image = BitmapFrame.Create(new Uri(valueString), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);

                // var bitmap = new BitmapImage();
                // bitmap.BeginInit();
                // bitmap.CacheOption = BitmapCacheOption.OnLoad;
                // bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                // bitmap.DecodePixelWidth = 400;
                // bitmap.StreamSource = File.OpenRead(valueString);
                // bitmap.EndInit();

                return BitmapFrame.Create(LoadImageFile(valueString));
            }
            catch { return null; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static BitmapSource LoadImageFile(String path)
        {
            Rotation rotation = Rotation.Rotate0;
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BitmapFrame bitmapFrame = BitmapFrame.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                BitmapMetadata bitmapMetadata = bitmapFrame.Metadata as BitmapMetadata;

                if ((bitmapMetadata != null) && (bitmapMetadata.ContainsQuery(OrientationQuery)))
                {
                    object o = bitmapMetadata.GetQuery(OrientationQuery);

                    if (o != null)
                    {
                        switch ((ushort)o)
                        {
                            case 6:
                            {
                                rotation = Rotation.Rotate90;
                            }
                                break;
                            case 3:
                            {
                                rotation = Rotation.Rotate180;
                            }
                                break;
                            case 8:
                            {
                                rotation = Rotation.Rotate270;
                            }
                                break;
                        }
                    }
                }
            }

            BitmapImage _image = new BitmapImage();
            _image.BeginInit();
            _image.UriSource = new Uri(path);
            _image.Rotation = rotation;
            _image.DecodePixelWidth = 400;
            _image.EndInit();
            _image.Freeze();

            return _image;
        }
    }
}
