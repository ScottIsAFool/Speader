using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Speader.Model;

namespace Speader.Controls
{
    public class ThemeFlyoutMenuItem : MenuFlyoutItem
    {
        public static readonly DependencyProperty HideTickProperty = DependencyProperty.Register(
            "HideTick", typeof (bool), typeof (ThemeFlyoutMenuItem), new PropertyMetadata(default(bool)));

        public bool HideTick
        {
            get { return (bool) GetValue(HideTickProperty); }
            set { SetValue(HideTickProperty, value); }
        }

        public static readonly DependencyProperty RequiredThemeProperty = DependencyProperty.Register(
            "RequiredTheme", typeof (Theme), typeof (ThemeFlyoutMenuItem), new PropertyMetadata(default(Theme), OnThemeChanged));

        public Theme RequiredTheme
        {
            get { return (Theme) GetValue(RequiredThemeProperty); }
            set { SetValue(RequiredThemeProperty, value); }
        }

        public static readonly DependencyProperty ActualThemeProperty = DependencyProperty.Register(
            "ActualTheme", typeof (Theme), typeof (ThemeFlyoutMenuItem), new PropertyMetadata(default(Theme), OnThemeChanged));

        public Theme ActualTheme
        {
            get { return (Theme) GetValue(ActualThemeProperty); }
            set { SetValue(ActualThemeProperty, value); }
        }

        public static readonly DependencyProperty TickOpacityProperty = DependencyProperty.Register(
            "TickOpacity", typeof (double), typeof (ThemeFlyoutMenuItem), new PropertyMetadata(default(Visibility)));

        public double TickOpacity
        {
            get { return (double) GetValue(TickOpacityProperty); }
            set { SetValue(TickOpacityProperty, value); }
        }

        public static readonly DependencyProperty BylineProperty = DependencyProperty.Register(
            "Byline", typeof (string), typeof (ThemeFlyoutMenuItem), new PropertyMetadata(default(string)));

        public string Byline
        {
            get { return (string) GetValue(BylineProperty); }
            set { SetValue(BylineProperty, value); }
        }

        private static void OnThemeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = sender as ThemeFlyoutMenuItem;
            if (item == null) return;

            item.TickOpacity = item.ActualTheme == item.RequiredTheme && !item.HideTick ? 1 : 0;
            item.Byline = App.Loader.GetString(string.Format("Theme{0}Byline", item.RequiredTheme));
        }
    }
}
