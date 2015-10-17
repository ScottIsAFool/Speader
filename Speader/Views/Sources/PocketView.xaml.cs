using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Speader.Model;

namespace Speader.Views.Sources
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PocketView 
    {
        public PocketView()
        {
            InitializeComponent();

            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.StopPullToRefreshMsg))
                {
                    BoundListBox.StopPullToRefreshLoading(true);
                }
            });

            NormalTileImage.SetTile(false, SourceProvider.Pocket);
            TransparentTileImage.SetTile(true, SourceProvider.Pocket);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameters = e.Parameter as NavigationParameters;
            if (parameters != null && parameters.ShowHomeButton)
            {
                AppBarHome.Visibility = Visibility.Visible;
            }

            base.OnNavigatedTo(e);
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await SaveTileImage(NormalTileImage, TransparentTileImage);
        }
    }
}
