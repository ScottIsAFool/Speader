using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Speader.Model;

namespace Speader.Controls
{
    public class FontFlyoutMenuItem : MenuFlyoutItem
    {
        public static readonly DependencyProperty HideTickProperty = DependencyProperty.Register(
            "HideTick", typeof(bool), typeof(FontFlyoutMenuItem), new PropertyMetadata(default(bool)));

        public bool HideTick
        {
            get { return (bool)GetValue(HideTickProperty); }
            set { SetValue(HideTickProperty, value); }
        }

        public static readonly DependencyProperty RequiredFontProperty = DependencyProperty.Register(
            "RequiredFont", typeof(ReaderFont), typeof(FontFlyoutMenuItem), new PropertyMetadata(default(ReaderFont), OnActualFontChanged));

        public ReaderFont RequiredFont
        {
            get { return (ReaderFont)GetValue(RequiredFontProperty); }
            set { SetValue(RequiredFontProperty, value); }
        }

        public static readonly DependencyProperty ActualFontProperty = DependencyProperty.Register(
            "ActualFont", typeof(ReaderFont), typeof(FontFlyoutMenuItem), new PropertyMetadata(default(ReaderFont), OnActualFontChanged));

        public ReaderFont ActualFont
        {
            get { return (ReaderFont)GetValue(ActualFontProperty); }
            set { SetValue(ActualFontProperty, value); }
        }

        public static readonly DependencyProperty TickOpacityProperty = DependencyProperty.Register(
            "TickOpacity", typeof(double), typeof(FontFlyoutMenuItem), new PropertyMetadata(default(Visibility)));

        public double TickOpacity
        {
            get { return (double)GetValue(TickOpacityProperty); }
            set { SetValue(TickOpacityProperty, value); }
        }

        public FontFlyoutMenuItem()
        {
            ShowHideTick();
        }

        private static void OnActualFontChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = sender as FontFlyoutMenuItem;
            if (item == null) return;

            item.ShowHideTick();
        }

        private void ShowHideTick()
        {
            TickOpacity = ActualFont == RequiredFont && !HideTick ? 1 : 0;
            switch (RequiredFont)
            {
                case ReaderFont.CourierNew:
                    Text = "Courier New";
                    break;
                case ReaderFont.Georgia:
                    Text = "Georgia";
                    break;
                case ReaderFont.Segoe:
                    Text = "Segoe";
                    break;
                case ReaderFont.OpenDyslexic:
                    Text = "OpenDyslexic";
                    break;
            }
        }
    }
}