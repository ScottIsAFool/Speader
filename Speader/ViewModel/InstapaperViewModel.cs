using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Insta.Portable;
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
    public class InstapaperViewModel : ProviderViewModelBase
    {
        private readonly IInstapaperClient _instapaperClient;

        /// <summary>
        /// Initializes a new instance of the InstapaperViewModel class.
        /// </summary>
        public InstapaperViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            IInstapaperClient instapaperClient,
            ICacheService cacheService,
            ITileService tileService,
            ReaderViewModel readerViewModel,
            FullPageViewModel fullPageViewModel)
            : base(navigationService, authenticationService, cacheService, tileService, readerViewModel, fullPageViewModel)
        {
            _instapaperClient = instapaperClient;
            IsPinned = TileService.IsInstapaperPinned;

            //EmailAddress = "mb@matthewbischoff.com";
            //Password = "YjJhZtE7nxwv8}";
            //EmailAddress = "scottisafool@gmail.com";
            //Password = "Coventry20!";
        }

        public string EmailAddress { get; set; }
        public string Password { get; set; }

        protected override async Task Connect()
        {
            SetProgressBar("Signing into Instapaper...");
            try
            {
                var response = await _instapaperClient.GetAuthTokenAsync(EmailAddress, Password);
                if (response != null)
                {
                    var user = await _instapaperClient.VerifyUserAsync();
                    if (user != null)
                    {
                        AuthenticationService.SetInstapaperUser(user.Response, response.Key, response.Secret);

                        await LoadData(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException("Connect()", ex);
            }

            SetProgressBar();
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

            var text = await GetArticleText(item.Id);
            var result = new ReaderItem();
            item.CopyItem(result);
            result.Text = text.StripHtmlTags();
            item.Text = result.Text.ToExcerpt();
            item.IsDownloaded = !string.IsNullOrEmpty(result.Text);
            await CacheService.SaveArticle(result).ConfigureAwait(false);

            return result;
        }

        private async Task<string> GetArticleText(string id)
        {
            try
            {
                var actualId = int.Parse(id);

                var article = await _instapaperClient.GetBookmarkContentAsync(actualId);
                return article;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        protected override async Task ViewLoaded()
        {
            await LoadData(false);
        }

        public override bool IsLoggedIn => AuthenticationService.IsLoggedIntoInstapaper;

        public override SourceProvider Provider => SourceProvider.Instapaper;

        protected override async Task LoadData(bool isRefresh)
        {
            if (!AuthenticationService.IsLoggedIntoInstapaper)
            {
                return;
            }

            var cacheResponse = await CacheService.GetInstapaperItemsFromCache();

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
                    Log.ErrorException("GoToWeb()", ex);
                }
            }

            DownloadArticles().ConfigureAwait(false);
        }

        private async Task GoToWeb()
        {
            var haveIds = Items.Select(x => int.Parse(x.Id));
            var theItems = await _instapaperClient.GetBookmarksAsync(Constants.MaxArticleCount, alreadyHave: haveIds);
            if (theItems == null
                || theItems.Error != null 
                || theItems.Response?.Bookmarks == null)
            {
                if (theItems?.Error != null)
                {

                }
                return;
            }

            var items = theItems.Response.Bookmarks.ToList();
            var deletedItems = theItems.Response.DeletedIds;
            if (items.IsNullOrEmpty())
            {
                return;
            }

            var noInstapaperItems = Items.IsNullOrEmpty();

            // Handle any deleted items
            if (deletedItems != null)
            {
                foreach (var item in deletedItems.ToList())
                {
                    if (noInstapaperItems)
                    {
                        break;
                    }

                    foreach (var r in Items.ToList())
                    {
                        if (r.Id == item.ToString())
                        {
                            Items.Remove(r);
                        }
                    }
                }
            }

            // Make sure articles aren't already in the list
            foreach (var a in items.ToList())
            {
                if (noInstapaperItems)
                {
                    break;
                }

                var existingItem = Items.FirstOrDefault(x => x.Id == a.Id.ToString());
                if (existingItem != null)
                {
                    var updatedItem = a.ToReaderItem();
                    updatedItem.CopyItem(existingItem);
                    items.Remove(a);
                }
            }

            var list = items.Select(x => x.ToReaderItem()).ToList();

            if (noInstapaperItems)
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

            await CacheService.SaveInstapaperItems(Items).ConfigureAwait(false);

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
            await Task.WhenAll(tasks).ContinueWith(async item =>
            {
                await CacheService.SaveInstapaperItems(Items);
            });
        }

        public override Task Logout()
        {
            CacheService.ClearInstapaperItems();
            AuthenticationService.SignOutFromInstapaper();
            return base.Logout();
        }

        protected override async Task Archive(ReaderItem item)
        {
            if (!AuthenticationService.IsLoggedIntoInstapaper) return;
            var id = int.Parse(item.Id);
            var failed = false;

            try
            {
                var archivedItem = await _instapaperClient.ArchiveBookmarkAsync(id);
                if (archivedItem == null || archivedItem.Response == null)
                {
                    failed = true;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException("ArchiveArticle", ex);
                failed = true;
            }

            if (!failed)
            {
                var itemToRemove = Items.FirstOrDefault(x => x.Id == item.Id);
                if (itemToRemove == null) return;

                Items.Remove(itemToRemove);

                CacheService.SaveInstapaperItems(Items).ConfigureAwait(false);

                await CacheService.DeleteArticle(item);
            }
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<ArchiveMessage>(this, async m =>
            {
                if (m.SourceProvider != SourceProvider.Instapaper) return;

                await Archive(m.Item);

                m.Action?.Invoke();
            });

            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.RefreshPinMsg))
                {
                    IsPinned = TileService.IsInstapaperPinned;
                }
            });

            base.WireMessages();
        }
    }
}