using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Cimbalino.Toolkit.Extensions;
using GalaSoft.MvvmLight.Messaging;
using Readability;
using Readability.Models;
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
    public class ReadabilityViewModel : ProviderViewModelBase
    {
        private readonly IReadabilityClient _readabilityClient;

        /// <summary>
        /// Initializes a new instance of the ReadabilityViewModel class.
        /// </summary>
        public ReadabilityViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            IReadabilityClient readabilityClient,
            ITileService tileService,
            ReaderViewModel readerViewModel,
            ICacheService cacheService,
            FullPageViewModel fullPageViewModel)
            : base(navigationService, authenticationService, cacheService, tileService, readerViewModel, fullPageViewModel)
        {
            _readabilityClient = readabilityClient;
            IsPinned = TileService.IsReadabilityPinned;
        }


        protected override async Task ViewLoaded()
        {
            await LoadData(false);
        }

        public override Task Logout()
        {
            CacheService.ClearReadabilityItems();
            AuthenticationService.SignOutFromReadability();
            return base.Logout();
        }

        protected override async Task Connect()
        {
            try
            {
                SetProgressBar("Talking to Readability...");
                await ReadabilityAuthentication();
            }
            catch (Exception ex)
            {
                Log.ErrorException("Connect()", ex);
            }

            SetProgressBar();
        }

        public override bool IsLoggedIn => AuthenticationService.IsLoggedIntoReadability;

        public override SourceProvider Provider => SourceProvider.Readability;

        protected override async Task LoadData(bool isRefresh)
        {
            if (!AuthenticationService.IsLoggedIntoReadability)
            {
                return;
            }

            var cacheResponse = await CacheService.GetReadabilityItemsFromCache();

            if (Items.IsNullOrEmpty())
            {
                Items = new ObservableCollection<ReaderItem>(cacheResponse.ReaderItems);
            }

            var goToWeb = cacheResponse.CacheExpired || !DataLoaded || isRefresh;

            if (goToWeb || Items.IsNullOrEmpty())
            {
                await GoToWeb();
            }

            DownloadArticles().ConfigureAwait(false);
        }

        private async Task GoToWeb()
        {
            var dateSince = CacheService.GetSinceDate(SourceProvider.Readability);
            var items = new List<Bookmark>();
            await GetItems(items, 1, dateSince);

            if (items.IsNullOrEmpty())
            {
                return;
            }

            var noReadabilityItems = Items.IsNullOrEmpty();

            // Remove archived items
            foreach (var item in items.Where(x => x.Archive).ToList())
            {
                if (noReadabilityItems)
                {
                    break;
                }

                foreach (var r in Items.ToList())
                {
                    if (r.Id == item.Article.Id)
                    {
                        Items.Remove(r);
                    }
                }

                items.Remove(item);
            }

            // Check for articles already in the list
            foreach (var a in items.ToList())
            {
                if (noReadabilityItems)
                {
                    break;
                }

                var existingItem = Items.FirstOrDefault(x => x.Id == a.Article.Id);
                if (existingItem != null)
                {
                    var updatedItem = a.Article.ToReaderItem();
                    updatedItem.CopyItem(existingItem);
                    items.Remove(a);
                }
            }

            var list = items.Select(x => x.Article.ToReaderItem()).ToList();

            if (noReadabilityItems)
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

            await CacheService.SaveReadabilityItems(Items).ConfigureAwait(false);

            DataLoaded = true;
        }

        private async Task GetItems(ICollection<Bookmark> items, int pageNumber, DateTime? dateSince)
        {
            while (true)
            {
                var articles = await _readabilityClient.GetBookmarksAsync(new Conditions { AddedSince = dateSince, Page = pageNumber, PerPage = 50 });
                if (articles == null)
                {
                    return;
                }

                items.AddRange(articles.Bookmarks);

                if (items.Count > Constants.MaxArticleCount)
                {
                    break;
                }

                if (items.Count < articles.Meta.ItemCountTotal)
                {
                    pageNumber = pageNumber + 1;
                    continue;
                }
                break;
            }
        }

        private async Task DownloadArticles()
        {
            var articlesToDownload = Items.Where(x => !x.IsDownloaded).ToList();
            if (articlesToDownload.IsNullOrEmpty())
            {
                return;
            }

            var tasks = articlesToDownload.Select(async x => await GetArticle(x, true));
            await Task.WhenAll(tasks).ContinueWith(async item =>
            {
                await CacheService.SaveReadabilityItems(Items);
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

            var response = await _readabilityClient.GetArticleAsync(item.Id);
            if (response != null)
            {
                var article = response.ToReaderItem();
                item.IsDownloaded = !string.IsNullOrEmpty(article.Text);
                await CacheService.SaveArticle(article).ConfigureAwait(false);
                return article;
            }

            return null;
        }

        private async Task ReadabilityAuthentication()
        {
            try
            {
                await _readabilityClient.GetRequestTokenAsync(Constants.Readability.CallBackUri);
            }
            catch (Exception ex)
            {
                Log.ErrorException("ReadabilityAuthentication()", ex);
                return;
            }

            var authUri = _readabilityClient.GenerateAuthenticationUri();

            WebAuthenticationBroker.AuthenticateAndContinue(authUri, new Uri(Constants.Readability.CallBackUri), new ValueSet(), WebAuthenticationOptions.None);
        }

        protected override async Task Archive(ReaderItem item)
        {
            if (!AuthenticationService.IsLoggedIntoReadability) return;
            var id = int.Parse(item.Id);
            var failed = false;

            try
            {
                await _readabilityClient.SetBookmarkArchiveStateAsync(id, true);
            }
            catch (Exception ex)
            {
                Log.ErrorException("ArchiveArticle", ex);
                failed = true;
            }

            if (!failed)
            {
                var itemToRemove = Items.FirstOrDefault(x => x.Id == item.Id);
                if (itemToRemove != null)
                {
                    Items.Remove(itemToRemove);
                }

                CacheService.SaveReadabilityItems(Items).ConfigureAwait(false);

                await CacheService.DeleteArticle(item);
            }
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.ReadabilityAuthMsg))
                {
                    var verification = (string)m.Sender;
                    await GetAuthenticationToken(verification);

                    if (!Items.IsNullOrEmpty())
                    {
                        await ViewLoaded();
                    }
                }

                if (m.Notification.Equals(Constants.Messages.RefreshPinMsg))
                {
                    IsPinned = TileService.IsReadabilityPinned;
                }
            });

            Messenger.Default.Register<ArchiveMessage>(this, async m =>
            {
                if (m.SourceProvider != SourceProvider.Readability) return;

                await Archive(m.Item);

                m.Action?.Invoke();
            });

            base.WireMessages();
        }

        private async Task GetAuthenticationToken(string verification)
        {
            try
            {
                SetProgressBar("Verifying user...");
                var response = await _readabilityClient.VerifyUserAsync(verification);
                if (response != null)
                {
                    var user = await _readabilityClient.GetProfileAsync();
                    if (user != null)
                    {
                        AuthenticationService.SetReadabilityUser(user, response.Key, response.Secret);

                        await LoadData(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException("GetAuthenticationToken", ex);
            }

            SetProgressBar();
        }
    }
}