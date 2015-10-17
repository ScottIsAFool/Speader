using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Speader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebView
    {
        public WebView()
        {
            InitializeComponent();
            Loaded += (sender, args) => AddressBox.Focus(FocusState.Keyboard);
        }

        private void AppBarBack_OnClick(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoBack)
            {
                Browser.GoBack();
            }
        }

        private void StopRefreshAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            var isLoading = (bool)StopRefreshAppBarButton.Tag;
            if (isLoading)
            {
                Browser.Stop();
            }
            else
            {
                Browser.Refresh();
            }
        }

        private void AppBarForward_OnClick(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoForward)
            {
                Browser.GoForward();
            }
        }

        private void AddressBox_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Browser.Focus(FocusState.Keyboard);
            }
        }
    }
}
