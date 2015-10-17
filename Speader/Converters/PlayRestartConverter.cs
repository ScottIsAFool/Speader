using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Speader.Converters
{
    public class PlayRestartConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isLoading = (bool)value;

            return isLoading ? new SymbolIcon(Symbol.Refresh) : new SymbolIcon(Symbol.Play);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}