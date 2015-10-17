using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Speader.Converters
{
    public class StopRefreshConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isLoading = (bool) value;

            return isLoading ? new SymbolIcon(Symbol.Stop) : new SymbolIcon(Symbol.Refresh);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
