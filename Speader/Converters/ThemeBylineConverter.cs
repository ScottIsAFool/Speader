using System;
using Windows.UI.Xaml.Data;
using Speader.Model;

namespace Speader.Converters
{
    public class ThemeBylineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var theme = (Theme)value;
            var key = $"Theme{theme}Byline";
            return App.Loader.GetString(key);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
