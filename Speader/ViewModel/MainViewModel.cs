using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Ailon.WP.Utils;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Speader.Extensions;
using Speader.Interfaces;
using Speader.Messages;
using Speader.Model;
using Speader.Views;
using InstapaperView = Speader.Views.Sources.InstapaperView;
using LocalView = Speader.Views.Sources.LocalView;
using PocketView = Speader.Views.Sources.PocketView;
using ReadabilityView = Speader.Views.Sources.ReadabilityView;
using WebView = Windows.UI.Xaml.Controls.WebView;

namespace Speader.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICacheService _cacheService;
        private readonly IReaderHelper _readerHelper;
        private readonly PocketViewModel _pocketViewModel;
        private readonly ReadabilityViewModel _readabilityViewModel;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            ICacheService cacheService,
            IReaderHelper readerHelper,
            PocketViewModel pocketViewModel,
            ReadabilityViewModel readabilityViewModel)
        {
            _navigationService = navigationService;
            _authenticationService = authenticationService;
            _cacheService = cacheService;
            _readerHelper = readerHelper;
            _pocketViewModel = pocketViewModel;
            _readabilityViewModel = readabilityViewModel;

#if WINDOWS_PHONE_APP
            var deviceInfo = new EasClientDeviceInformation();
            var phone = PhoneNameResolver.Resolve(deviceInfo.SystemManufacturer, deviceInfo.SystemProductName);
            DeviceName = $"{deviceInfo.FriendlyName} ({phone.CanonicalModel})";
#endif

        }

#if WINDOWS_PHONE_APP
        public ObservableCollection<ReaderItem> InProgressItems { get; set; }
        public ObservableCollection<ReaderItem> RecentItems { get; set; }
        public string DeviceName { get; set; }

        public int SelectedIndex { get; set; }
        public bool IsSaving { get; set; }

        public RelayCommand MainViewLoadedCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    LoadInProgressItems().ConfigureAwait(false);
                    LoadRecentItems().ConfigureAwait(false);
                });
            }
        }

        public RelayCommand NavigateToLocalCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<LocalView>()); }
        }

        public RelayCommand NavigateToPocketCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (_authenticationService.IsLoggedInToPocket)
                    {
                        _navigationService.Navigate<PocketView>();
                    }
                    else
                    {
                        _pocketViewModel.ConnectCommand.Execute(null);
                    }
                });
            }
        }

        public RelayCommand NavigateToReadabilityCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (_authenticationService.IsLoggedIntoReadability)
                    {
                        _navigationService.Navigate<ReadabilityView>();
                    }
                    else
                    {
                        _readabilityViewModel.ConnectCommand.Execute(null);
                    }
                });
            }
        }

        public RelayCommand NavigateToInstapaperCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<InstapaperView>()); }
        }

        public RelayCommand NavigateToSettingsCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<SettingsView>()); }
        }

        public RelayCommand NavigateToWebCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<WebView>()); }
        }

        public RelayCommand NavigateToAboutCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<AboutView>()); }
        }

        public RelayCommand NavigateToHelpCommand
        {
            get { return new RelayCommand(() => _navigationService.Navigate<HelpView>()); }
        }

        public RelayCommand<ReaderItem> RemoveFromRecentListCommand
        {
            get
            {
                return new RelayCommand<ReaderItem>(async item =>
                {
                    var response = await _cacheService.GetRecentItemsFromCache();
                    var itemToRemove = response.ReaderItems.FirstOrDefault(x => x.Id == item.Id);
                    if (itemToRemove != null)
                    {
                        response.ReaderItems.Remove(itemToRemove);
                        RecentItems.Remove(item);
                        await _cacheService.SaveRecentItems(RecentItems).ConfigureAwait(false);
                    }
                    else
                    {
                        RecentItems.Remove(item);
                    }
                });
            }
        }

        public RelayCommand<ReaderItem> RemoveFromInProgressListCommand
        {
            get
            {
                return new RelayCommand<ReaderItem>(async item =>
                {
                    var response = await _cacheService.GetInProgressItemsFromCache();
                    var itemToRemove = response.ReaderItems.FirstOrDefault(x => x.Id == item.Id);
                    if (itemToRemove != null)
                    {
                        response.ReaderItems.Remove(itemToRemove);
                        InProgressItems.Remove(item);
                        await _cacheService.SaveInProgressItems(InProgressItems).ConfigureAwait(false);
                    }
                    else
                    {
                        InProgressItems.Remove(item);
                    }
                });
            }
        }

        private async Task LoadInProgressItems()
        {
            var response = await _cacheService.GetInProgressItemsFromCache();

            if (InProgressItems.IsNullOrEmpty())
            {
                InProgressItems = new ObservableCollection<ReaderItem>(response.ReaderItems);
                return;
            }

            foreach (var item in response.ReaderItems)
            {
                var existingItem = InProgressItems.FirstOrDefault(x => x.Id == item.Id);
                if (existingItem != null)
                {
                    item.CopyItem(existingItem);
                }
                else
                {
                    InProgressItems.Add(item);
                }
            }
        }

        private async Task LoadRecentItems()
        {
            var response = await _cacheService.GetRecentItemsFromCache();

            if (RecentItems.IsNullOrEmpty())
            {
                RecentItems = new ObservableCollection<ReaderItem>(response.ReaderItems);
                return;
            }

            foreach (var item in response.ReaderItems)
            {
                var existingItem = RecentItems.FirstOrDefault(x => x.Id == item.Id);
                if (existingItem != null)
                {
                    item.CopyItem(existingItem);
                }
                else
                {
                    RecentItems.Add(item);
                }
            }
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<UriSchemeMessage>(this, async m =>
            {
                if (m.SchemeType != SchemeType.Save)
                {
                    return;
                }

                IsSaving = true;

                try
                {
                    await _readerHelper.HandleProtocolMessage(m);
                }
                catch (Exception ex)
                {
                    Log.ErrorException("Saving from Scheme", ex);
                }

                IsSaving = false;
            });
        }
#endif
    }
}