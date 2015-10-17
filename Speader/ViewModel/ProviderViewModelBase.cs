using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Speader.Interfaces;
using Speader.Messages;
using Speader.Model;
using Speader.Views;

namespace Speader.ViewModel
{
    public abstract class ProviderViewModelBase : ViewModelBase
    {
        protected readonly INavigationService NavigationService;
        protected readonly IAuthenticationService AuthenticationService;
        protected readonly ICacheService CacheService;
        protected readonly ITileService TileService;
        protected readonly ReaderViewModel ReaderViewModel;
        protected readonly FullPageViewModel FullPageViewModel;

        protected bool DataLoaded;

        protected ProviderViewModelBase(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            ICacheService cacheService,
            ITileService tileService,
            ReaderViewModel readerViewModel,
            FullPageViewModel fullPageViewModel)
        {
            NavigationService = navigationService;
            AuthenticationService = authenticationService;
            CacheService = cacheService;
            TileService = tileService;
            ReaderViewModel = readerViewModel;
            FullPageViewModel = fullPageViewModel;
        }

        public abstract bool IsLoggedIn { get; }

        public abstract SourceProvider Provider { get; }

        public bool IsPinned { get; set; }

        public ObservableCollection<ReaderItem> Items { get; set; }

        public RelayCommand PinUnpinCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!IsPinned)
                    {
                        var result = TileService.PinSource(Provider).ConfigureAwait(false);
                        IsPinned = true;
                    }
                    else
                    {
                        var result = TileService.UnpinSource(Provider).ConfigureAwait(false);
                        IsPinned = false;
                    }
                });
            }
        }

        public RelayCommand ViewLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await ViewLoaded();
                });
            }
        }

        public RelayCommand ConnectCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await Connect();
                });
            }
        }

        public RelayCommand LogoutCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await Logout();
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await Refresh();
                    Messenger.Default.Send(new NotificationMessage(Constants.Messages.StopPullToRefreshMsg));
                });
            }
        }

        public RelayCommand<ReaderItem> ArchiveCommand
        {
            get
            {
                return new RelayCommand<ReaderItem>(async item =>
                {
                    await Archive(item);
                });
            }
        }

        public RelayCommand<ReaderItem> FullPageCommand
        {
            get
            {
                return new RelayCommand<ReaderItem>(async item =>
                {
                    await FullPage(item);
                });
            }
        }

        public RelayCommand<ReaderItem> ReaderPageCommand
        {
            get
            {
                return new RelayCommand<ReaderItem>(async item =>
                {
                    await Reader(item);
                });
            }
        }

        protected virtual Task LoadData(bool isRefresh) { return null; }

        protected virtual Task Connect()
        {
            return null;
        }

        public virtual async Task Logout()
        {
            DataLoaded = false;
            Items = new ObservableCollection<ReaderItem>();
        }

        protected virtual Task Refresh()
        {
            return LoadData(true);
        }

        protected virtual Task ViewLoaded()
        {
            return null;
        }

        protected virtual Task<ReaderItem> GetArticle(ReaderItem item, bool isRefresh = false)
        {
            return null;
        }

        protected virtual async Task FullPage(ReaderItem item)
        {
            var article = await GetArticle(item);
            if (article == null)
            {
                return;
            }

            FullPageViewModel.ReaderItem = article;
            NavigationService.Navigate<FullPageView>();
        }

        protected virtual async Task Reader(ReaderItem item)
        {
            var article = await GetArticle(item);
            if (article == null)
            {
                return;
            }

            ReaderViewModel.SetReaderItem(article);
            NavigationService.Navigate<ReaderView>();
        }

        protected virtual Task Archive(ReaderItem item)
        {
            return null;
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<TileMessage>(this, m =>
            {
                if (m.SourceProvider == Provider)
                {
                    //PinUnpinCommand.Execute(m.Tile);
                }
            });
        }
    }
}
