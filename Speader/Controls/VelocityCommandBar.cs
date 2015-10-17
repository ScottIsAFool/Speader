using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Speader.Extensions;

namespace Speader.Controls
{
    public class SpeaderCommandBar : CommandBar
    {
        public SpeaderCommandBar()
        {
            Refresh();
        }

        protected override void OnClosed(object e)
        {
            Refresh();
        }

        protected override void OnOpened(object e)
        {
            Refresh(true);
        }

        public void Refresh(bool isOpen = false)
        {
            if (isOpen)
            {
                Background = Application.Current.GetThemeResource<SolidColorBrush>("AppBarOpenBrush");
                Foreground = Application.Current.GetThemeResource<SolidColorBrush>("AltBackgroundBrush");
            }
            else
            {
                Background = Application.Current.GetThemeResource<SolidColorBrush>("AltBackgroundBrush");
                Foreground = Application.Current.GetThemeResource<SolidColorBrush>("AppBarOpenBrush");
            }
        }
    }
}
