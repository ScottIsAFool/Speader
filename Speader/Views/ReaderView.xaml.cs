using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;

namespace Speader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReaderView 
    {
        public ReaderView()
        {
            InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;
        }

        private void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs backPressedEventArgs)
        {
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.ReaderViewLeftMsg));
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(MenuGrid);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.SaveSettingsMsg));
            base.OnNavigatingFrom(e);
        }
    }
}
