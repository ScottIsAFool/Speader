using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Speader.Converters
{
    public class BooleanToLocalisedStringConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty TrueValueProperty = DependencyProperty.Register(
            "TrueValue", typeof (string), typeof (BooleanToLocalisedStringConverter), new PropertyMetadata(default(string)));

        public string TrueValue
        {
            get { return (string) GetValue(TrueValueProperty); }
            set { SetValue(TrueValueProperty, value); }
        }

        public static readonly DependencyProperty FalseValueProperty = DependencyProperty.Register(
            "FalseValue", typeof (string), typeof (BooleanToLocalisedStringConverter), new PropertyMetadata(default(string)));

        public string FalseValue
        {
            get { return (string) GetValue(FalseValueProperty); }
            set { SetValue(FalseValueProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = (bool) value;
            return item 
                ? StringToDisplay(TrueValue) 
                : StringToDisplay(FalseValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private static string StringToDisplay(string originalString)
        {
            if (string.IsNullOrEmpty(originalString)) return originalString;
            var localisedString = App.Loader.GetString(originalString);
            return string.IsNullOrEmpty(localisedString) ? originalString : localisedString;
        }
    }
}
