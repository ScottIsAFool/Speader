using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Speader.Model;

namespace Speader.Converters
{
    public class ReaderFontConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var font = (ReaderFont)value;
            switch (font)
            {
                case ReaderFont.CourierNew:
                    return new FontFamily("Courier New");
                case ReaderFont.Georgia:
                    return new FontFamily("Georgia");
                case ReaderFont.OpenDyslexic:
                    return new FontFamily("/Fonts/OpenDyslexic-Regular.ttf#OpenDyslexic");
                default:
                    return new FontFamily("Segoe WP SemiLight");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
