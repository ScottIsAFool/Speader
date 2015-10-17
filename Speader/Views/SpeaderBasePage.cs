using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using ScottIsAFool.WindowsPhone.Logging;
using Speader.Common;
using Speader.Controls;
using Speader.Interfaces;
using Speader.Model;
using Speader.Services;
using Speader.ViewModel;
using ThemeManagerRt;

namespace Speader.Views
{
    public class SpeaderBasePage : ThemeEnabledPage
    {
        private readonly NavigationHelper _navigationHelper;
        protected readonly ILog Logger;

        public SpeaderBasePage()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelperLoadState;
            _navigationHelper.SaveState += NavigationHelperSaveState;
            Logger = new WinLogger(GetType().FullName);
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper => _navigationHelper;

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        protected virtual void NavigationHelperLoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        protected virtual void NavigationHelperSaveState(object sender, SaveStateEventArgs e)
        {
        }

        protected virtual void InitialiseOnBack()
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Logger.Info("Navigated to {0}", GetType().FullName);

            if (e.NavigationMode == NavigationMode.Back)
            {
                InitialiseOnBack();
            }
            else
            {
                var parameters = e.Parameter as NavigationParameters;
                if (parameters != null && parameters.ClearBackstack)
                {
                    Logger.Info("Clearing backstack");
                    Frame.SetNavigationState("1,0");
                }
            }

            if (BottomAppBar != null)
            {
                var appBar = BottomAppBar as SpeaderCommandBar;
                if (appBar != null)
                {
                    appBar.Refresh();
                }
            }

            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Logger.Info("Navigated from {0}", GetType().FullName);
            _navigationHelper.OnNavigatedFrom(e);
        }

        protected void AppBarHome_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        protected async Task SaveTileImage(SourceTile normalTileImage, SourceTile transparentTileImage)
        {
            var settings = SimpleIoc.Default.GetInstance<ISettingsService>();
            var storage = SimpleIoc.Default.GetInstance<IStorageService>();

            var tileService = new TileService(storage, settings); 
            var vm = DataContext as ProviderViewModelBase;
            if (vm != null)
            {
                var file = tileService.GetTileFile(vm.Provider.ToString(), true);
                await tileService.SaveVisualElementToFile(transparentTileImage, file, 360, 360);

                file = tileService.GetTileFile(vm.Provider.ToString(), false);
                await tileService.SaveVisualElementToFile(normalTileImage, file, 360, 360);

                vm.PinUnpinCommand.Execute(null);
            }
        }
    }
}
