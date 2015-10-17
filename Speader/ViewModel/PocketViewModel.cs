using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using GalaSoft.MvvmLight.Messaging;
using PocketSharp;
using Speader.Extensions;
using Speader.Interfaces;
using Speader.Messages;
using Speader.Model;

namespace Speader.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class PocketViewModel : ProviderViewModelBase
    {
        private readonly IPocketClient _pocketClient;

        /// <summary>
        /// Initializes a new instance of the PocketViewModel class.
        /// </summary>
        public PocketViewModel(
            INavigationService navigationService,
            IPocketClient pocketClient,
            IAuthenticationService authenticationService,
            ICacheService cacheService,
            ITileService tileService,
            ReaderViewModel readerViewModel,
            FullPageViewModel fullPageViewModel)
            : base(navigationService, authenticationService, cacheService, tileService, readerViewModel, fullPageViewModel)
        {
            _pocketClient = pocketClient;

            IsPinned = TileService.IsPocketPinned;
        }

        protected override async Task Connect()
        {
            SetProgressBar("Talking to Pocket...");

            DataLoaded = false;
            await PocketAuthentication();

            SetProgressBar();
        }

        protected override async Task ViewLoaded()
        {
            await LoadData(false);
        }

        public override Task Logout()
        {
            CacheService.ClearPocketItems();
            AuthenticationService.SignOutFromPocket();
            return base.Logout();
        }
        
        private async Task PocketAuthentication()
        {
            try
            {
                var code = await _pocketClient.GetRequestCode();
            }
            catch (Exception ex)
            {
                Log.ErrorException("PocketAuthentication", ex);
                return;
            }

            var authUri = _pocketClient.GenerateAuthenticationUri();

#if WINDOWS_PHONE_APP
            WebAuthenticationBroker.AuthenticateAndContinue(authUri, new Uri(Constants.Pocket.CallBackUri), new ValueSet(), WebAuthenticationOptions.None);
#else
            var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, authUri);

            await GetAuthenticationToken();
#endif
        }

        private async Task GetAuthenticationToken()
        {
            try
            {
                SetProgressBar("Verifying user...");
                var user = await _pocketClient.GetUser();
                if (user != null)
                {
                    AuthenticationService.SetPocketUser(user);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException("GetAuthenticationToken", ex);
            }

            SetProgressBar();
        }

        public override bool IsLoggedIn => AuthenticationService.IsLoggedInToPocket;

        public override SourceProvider Provider => SourceProvider.Pocket;

        protected override async Task LoadData(bool isRefresh)
        {
            if (!AuthenticationService.IsLoggedInToPocket)
            {
                return;
            }

            var cacheResponse = await CacheService.GetPocketItemsFromCache();

            if (Items.IsNullOrEmpty())
            {
                Items = new ObservableCollection<ReaderItem>(cacheResponse.ReaderItems);
            }

            var goToWeb = cacheResponse.CacheExpired || !DataLoaded || isRefresh || Items.IsNullOrEmpty();

            if (goToWeb)
            {
                try
                {
                    await GoToWeb();
                }
                catch (Exception ex)
                {
                    Log.ErrorException("GoToWeb", ex);
                }
            }

            DownloadArticles().ConfigureAwait(false);
        }

        private async Task GoToWeb()
        {
            var dateSince = CacheService.GetSinceDate(SourceProvider.Pocket);
            var theItems = await _pocketClient.Get(since: dateSince, count: Constants.MaxArticleCount);
            var items = theItems.Where(x => !x.IsVideo && !x.IsVideo).Select(x => x.Extend()).ToList();
            if (items.IsNullOrEmpty())
            {
                return;
            }

            var noPocketItems = Items.IsNullOrEmpty();

            // Handle deleted/archived Items
            foreach (var item in items.Where(x => x.IsDeleted || x.IsArchive).ToList())
            {
                if (noPocketItems)
                {
                    break;
                }

                foreach (var r in Items.ToList())
                {
                    if (r.Id == item.ID)
                    {
                        Items.Remove(r);
                    }
                }

                items.Remove(item);
            }

            // Make sure articles aren't already in the list
            foreach (var a in items.ToList())
            {
                if (noPocketItems)
                {
                    break;
                }

                var existingItem = Items.FirstOrDefault(x => x.Id == a.ResolvedId);
                if (existingItem != null)
                {
                    var updatedItem = a.ToReaderItem();
                    updatedItem.CopyItem(existingItem);
                    items.Remove(a);
                }
            }

            var list = items.Select(x => x.ToReaderItem()).ToList();

            // Now we actually add any new items
            if (noPocketItems)
            {
                Items = new ObservableCollection<ReaderItem>(list.Take(Constants.MaxArticleCount));
            }
            else
            {
                var i = 0;
                foreach (var item in list)
                {
                    Items.Insert(i, item);
                    if (Items.Count > Constants.MaxArticleCount - 1)
                    {
                        Items.RemoveAt(Constants.MaxArticleCount);
                    }

                    i++;
                }
            }

            await CacheService.SavePocketItems(Items).ConfigureAwait(false);

            DataLoaded = true;
        }

        private async Task DownloadArticles()
        {
            var articlesToDownload = Items.Where(x => !x.IsDownloaded).ToList();
            if (articlesToDownload.IsNullOrEmpty())
            {
                return;
            }

            var tasks = articlesToDownload.Select(async x =>
            {
                try
                {
                    await GetArticle(x, true);
                }
                catch (Exception ex)
                {
                    Log.ErrorException("DownloadArticles", ex);
                }
            });
            await Task.WhenAll(tasks).ContinueWith(async items =>
            {
                await CacheService.SavePocketItems(Items).ConfigureAwait(false);
            });
        }

        protected override async Task<ReaderItem> GetArticle(ReaderItem item, bool isRefresh = false)
        {
            if (item == null)
            {
                return null;
            }

            if (!isRefresh)
            {
                var article = await CacheService.GetArticle(item);
                if (article != null)
                {
                    return article;
                }
            }

            try
            {
                var response = await _pocketClient.GetArticle(new Uri(item.Url, UriKind.Absolute), forceRefresh: isRefresh);
                if (response != null)
                {
                    var article = response.ToReaderItem();
                    item.IsDownloaded = true;
                    await CacheService.SaveArticle(article).ConfigureAwait(false);
                    return article;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException("GetArticle", ex);
            }

            return null;
        }

        protected override async Task Archive(ReaderItem item)
        {
            if (!AuthenticationService.IsLoggedInToPocket) return;
            var failed = false;
            try
            {
                if (!await _pocketClient.Archive(item.Id))
                {
                    failed = true;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException("PocketArchiveMessage", ex);
                failed = true;
            }

            if (!failed)
            {
                var itemToDelete = Items.FirstOrDefault(x => x.Id == item.Id);
                if (itemToDelete == null) return;

                Items.Remove(itemToDelete);
                CacheService.SavePocketItems(Items).ConfigureAwait(false);

                await CacheService.DeleteArticle(item);
            }
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.PocketAuthMsg))
                {
                    await GetAuthenticationToken();

                    await LoadData(false);
                }

                if (m.Notification.Equals(Constants.Messages.RefreshPinMsg))
                {
                    IsPinned = TileService.IsPocketPinned;
                }
            });

            Messenger.Default.Register<ArchiveMessage>(this, async m =>
            {
                if (m.SourceProvider != SourceProvider.Pocket) return;

                await Archive(m.Item);

                m.Action?.Invoke();
            });

            base.WireMessages();
        }
    }
}