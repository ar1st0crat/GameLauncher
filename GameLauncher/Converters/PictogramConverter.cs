using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using GameLauncher.Model;

namespace GameLauncher.Converters
{
    public class PictogramConverter : IValueConverter
    {
        private const string NO_PICTURE = @"../images/question-mark.png";
        
        /// <summary>
        /// Basic conversion:           GAME       ->   WriteableBitmap (or NO_PICTURE)
        ///                         OR
        ///                             ImagePath  ->   WriteableBitmap (or NO_PICTURE)
        /// </summary>
        /// <param name="parameter">The height of a thumbnail image, or null if the original image should be returned</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imagePath = value as string;

            Game game = value as Game;

            // if the object of conversion is GAME
            if (game == null)
            {
                if (imagePath == null)
                {
                    return "";
                }
            }
            else
            {
                imagePath = game.ImagePath;
            }

            if (!System.IO.File.Exists(imagePath))
            {
                return NO_PICTURE;
            }

            if (parameter == null)
            {
                return imagePath;
            }
            
            // optimization: reduce original image (set DecodePixelHeight property)
            // and release the original image by returning new WriteableBitmap
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bi.DecodePixelHeight = (int)parameter;
            bi.UriSource = new Uri(imagePath);
            bi.EndInit();
            bi.Freeze();

            return new WriteableBitmap(bi);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
