using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using GameLauncher.Model;

namespace GameLauncher.Converters
{
    public class PictogramStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var games = value[0] as List<Game>;
            var selectedGame = value[1] as Game;

            Uri resourceLocator = new Uri("/GameLauncher;component/Styles/PictogramStyles.xaml", UriKind.Relative);

            ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocator);

            if (games == null || selectedGame == null || (int)parameter >= games.Count)
            {
                return resourceDictionary["PictogramStyle"] as Style;
            }

            if (selectedGame.Id == games[(int) parameter].Id)
            {
                return resourceDictionary["SelectedPictogramStyle"] as Style;
            }

            return resourceDictionary["PictogramStyle"] as Style;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
