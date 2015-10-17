using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Speader.Converters
{
    public class PinUnpinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isPinned = (bool) value;

            return isPinned ? new SymbolIcon(Symbol.UnPin) : new SymbolIcon(Symbol.Pin);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}