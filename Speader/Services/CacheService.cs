using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using PropertyChanged;
using ScottIsAFool.WindowsPhone.Logging;
using Speader.Extensions;
using Speader.Interfaces;
using Speader.Model;

namespace Speader.Services
{
    [ImplementPropertyChanged]
    public class CacheService : ICacheService
    {
        private const string CacheFolder = "DataCache";
        private const string PocketCache = "Pocket_{0}";
        private const string ReadabilityCache = "Readability_{0}";
        private const string InstapaperCache = "Instapaper_{0}";
        private const string LocalCache = "LocalCache";
        private const string InProgressCache = "InProgressCache";
        private const string RecentCache = "RecentCache";
        private const string SinceDate = "Since_Date";
        private const string CountsFile = "Counts";

        private readonly IStorageService _storage;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILog _logger;

        private CacheSince _cacheSince;

        public CacheService(
            IStorageService storage,
            IAuthenticationService authenticationService,
            ILog logger)
        {
            _storage = storage;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        public CacheCount CacheCount { get; set; }

        public async Task StartService()
        {
            await CheckAndCreateCacheFolder();
            await CheckAndCreateSinceFile();
            await ReadCountsFromStorage();
        }

        #region Pocket Methods
        public async Task<CacheResponse> GetPocketItemsFromCache()
        {
            return await GetItemsFromCache(SourceProvider.Pocket);
        }

        public async Task SavePocketItems(ObservableCollection<ReaderItem> items)
        {
            await WriteItemsToDisk(items, SourceProvider.Pocket);
            if (items.Any())
            {
                CacheCount.PocketCount = items.Count;
            }
            else
            {
                CacheCount.PocketCount = null;
            }
            await WriteCountsToStorage().ConfigureAwait(false);
        }

        public async Task ClearPocketItems()
        {
            var filename = GetFileName(GetCacheFile(SourceProvider.Pocket));
            if (await _storage.Local.FileExistsAsync(filename))
            {
                await _storage.Local.DeleteFileAsync(filename);
            }

            CacheCount.PocketCount = null;
            _cacheSince.PocketSince = null;

            WriteCountsToStorage().ConfigureAwait(false);
            WriteSinceToStorage().ConfigureAwait(false);
        }

        #endregion

        #region Readability Methods
        public async Task<CacheResponse> GetReadabilityItemsFromCache()
        {
            return await GetItemsFromCache(SourceProvider.Readability);
        }

        public async Task SaveReadabilityItems(ObservableCollection<ReaderItem> items)
        {
            await WriteItemsToDisk(items, SourceProvider.Readability);

            if (items.Any())
            {
                CacheCount.ReadabilityCount = items.Count;
            }
            else
            {
                CacheCount.ReadabilityCount = null;
            }
            await WriteCountsToStorage().ConfigureAwait(false);
        }

        public async Task ClearReadabilityItems()
        {
            var filename = GetFileName(GetCacheFile(SourceProvider.Readability));
            if (await _storage.Local.FileExistsAsync(filename))
            {
                await _storage.Local.DeleteFileAsync(filename);
            }

            CacheCount.ReadabilityCount = null;
            _cacheSince.ReadabilitySince = null;

            WriteCountsToStorage().ConfigureAwait(false);
            WriteSinceToStorage().ConfigureAwait(false);
        }

        #endregion

        #region Local items Methods

        public async Task<CacheResponse> GetLocalItemsFromCache()
        {
            return await GetItemsFromCache(SourceProvider.Local);
        }

        public async Task SaveLocalItems(IList<ReaderItem> items)
        {
            await WriteItemsToDisk(items, SourceProvider.Local);

            if (!items.IsNullOrEmpty())
            {
                CacheCount.LocalCount = items.Count;
            }
            else
            {
                CacheCount.LocalCount = null;
            }

            WriteCountsToStorage().ConfigureAwait(false);
        }

        public async Task ClearLocalItems()
        {
            var filename = GetFileName(GetCacheFile(SourceProvider.Local));
            if (await _storage.Local.FileExistsAsync(filename))
            {
                await _storage.Local.DeleteFileAsync(filename);
            }
        }
        #endregion

        #region Instapaper Methods
        public async Task<CacheResponse> GetInstapaperItemsFromCache()
        {
            return await GetItemsFromCache(SourceProvider.Instapaper);
        }

        public async Task SaveInstapaperItems(ObservableCollection<ReaderItem> items)
        {
            await WriteItemsToDisk(items, SourceProvider.Instapaper);

            if (items.Any())
            {
                CacheCount.InstapaperCount = items.Count;
            }
            else
            {
                CacheCount.InstapaperCount = null;
            }

            WriteCountsToStorage().ConfigureAwait(false);
        }

        public async Task ClearInstapaperItems()
        {
            var filename = GetFileName(GetCacheFile(SourceProvider.Instapaper));
            if (await _storage.Local.FileExistsAsync(filename))
            {
                await _storage.Local.DeleteFileAsync(filename);
            }

            CacheCount.InstapaperCount = null;
            _cacheSince.InstapaperSince = null;

            WriteCountsToStorage().ConfigureAwait(false);
            WriteSinceToStorage().ConfigureAwait(false);
        }
        #endregion

        #region In Progress Methods

        public async Task<CacheResponse> GetInProgressItemsFromCache()
        {
            return await GetItemsFromCache(SourceProvider.InProgress);
        }

        public async Task SaveInProgressItems(IList<ReaderItem> items)
        {
            await WriteItemsToDisk(items, SourceProvider.InProgress);
        }

        public async Task ClearInProgressItems()
        {
            var filename = GetFileName(GetCacheFile(SourceProvider.InProgress));
            if (await _storage.Local.FileExistsAsync(filename))
            {
                await _storage.Local.DeleteFileAsync(filename);
            }
        }

        #endregion

        #region Recent Methods

        public async Task<CacheResponse> GetRecentItemsFromCache()
        {
            return await GetItemsFromCache(SourceProvider.Recent);
        }

        public async Task SaveRecentItems(IList<ReaderItem> items)
        {
            await WriteItemsToDisk(items, SourceProvider.Recent);
        }

        public async Task ClearRecentItems()
        {
            var filename = GetFileName(GetCacheFile(SourceProvider.Recent));
            if (await _storage.Local.FileExistsAsync(filename))
            {
                await _storage.Local.DeleteFileAsync(filename);
            }
        }

        #endregion

        #region Article methods
        public async Task<ReaderItem> GetArticle(ReaderItem item)
        {
            var filename = GetFileName(item.InternalId);
            if (!await _storage.Local.FileExistsAsync(filename))
            {
                return null;
            }

            using (var file = await _storage.Local.OpenFileForReadAsync(filename))
            {
                using (var reader = new BinaryReader(file))
                {
                    var cachedItem = reader.ReadGeneric<ReaderItem>();
                    return cachedItem;
                }
            }
        }

        public async Task SaveArticle(ReaderItem item)
        {
            var filename = GetFileName(item.InternalId);

            using (var file = await _storage.Local.CreateFileAsync(filename))
            {
                using (var writer = new BinaryWriter(file))
                {
                    writer.Write(item);
                }
            }
        }

        public async Task DeleteArticle(ReaderItem item)
        {
            var filename = GetFileName(item.InternalId);

            if (await _storage.Local.FileExistsAsync(filename))
            {
                await _storage.Local.DeleteFileAsync(filename);
            }
        }
        #endregion

        public async Task ClearAllItems()
        {
            await ClearPocketItems();
            await ClearReadabilityItems();
            await ClearInstapaperItems();
            await ClearLocalItems();
            await InvalidateCache();
        }

        public async Task InvalidateCache()
        {
            _cacheSince = new CacheSince();
            WriteSinceToStorage().ConfigureAwait(false);
        }

        public DateTime? GetSinceDate(SourceProvider type)
        {
            switch (type)
            {
                case SourceProvider.Pocket:
                    return _cacheSince.PocketSince;
                case SourceProvider.Instapaper:
                    return _cacheSince.InstapaperSince;
                case SourceProvider.Readability:
                    return _cacheSince.ReadabilitySince;
                default:
                    return null;
            }
        }

        private async Task SetSinceDate(SourceProvider type, DateTime? date = null)
        {
            switch (type)
            {
                case SourceProvider.Pocket:
                    _cacheSince.PocketSince = date;
                    break;
                case SourceProvider.Instapaper:
                    _cacheSince.InstapaperSince = date;
                    break;
                case SourceProvider.Readability:
                    _cacheSince.ReadabilitySince = date;
                    break;
                default:
                    return;
            }

            await WriteSinceToStorage().ConfigureAwait(false);
        }

        public async Task ClearAllSinceDates()
        {
            var filename = GetSinceFileName();
            if (await _storage.Local.FileExistsAsync(filename))
            {
                await _storage.Local.DeleteFileAsync(filename);
            }
        }

        public CacheCount GetCounts()
        {
            return CacheCount;
        }

        private string GetCacheFile(SourceProvider type)
        {
            switch (type)
            {
                case SourceProvider.Pocket:
                    return string.Format(PocketCache, _authenticationService.LoggedInPocketUser.Username);
                case SourceProvider.Instapaper:
                    return string.Format(InstapaperCache, _authenticationService.LoggedInInstapaperUser.ID);
                case SourceProvider.Readability:
                    return string.Format(ReadabilityCache, _authenticationService.LoggedInReadabilityUser.Name);
                case SourceProvider.Local:
                    return LocalCache;
                case SourceProvider.InProgress:
                    return InProgressCache;
                case SourceProvider.Recent:
                    return RecentCache;
                default:
                    return null;
            }
        }

        private async Task WriteItemsToDisk(IList<ReaderItem> items, SourceProvider source)
        {
            var filename = GetFileName(GetCacheFile(source));

            using (var file = await _storage.Local.CreateFileAsync(filename))
            {
                using (var writer = new BinaryWriter(file))
                {
                    writer.WriteList(items);
                }
            }

            await SetSinceDate(source, DateTime.Now).ConfigureAwait(false);
        }

        private async Task<CacheResponse> GetItemsFromCache(SourceProvider source)
        {
            var filename = GetFileName(GetCacheFile(source));

            if (await _storage.Local.FileExistsAsync(filename))
            {
                var items = new List<ReaderItem>();
                using (var file = await _storage.Local.OpenFileForReadAsync(filename))
                {
                    using (var reader = new BinaryReader(file))
                    {
                        try
                        {
                            items = reader.ReadList<ReaderItem>();
                        }
                        catch
                        {
                        }
                    }
                }

                var expired = CacheHasExpired(source);

                return new CacheResponse(items ?? new List<ReaderItem>(), expired);
            }

            return new CacheResponse(new List<ReaderItem>(), false);
        }

        private static string GetFileName(string filename)
        {
            return $"{CacheFolder}\\{filename}";
        }

        private static string GetSinceFileName()
        {
            return GetFileName(SinceDate);
        }

        private async Task CheckAndCreateCacheFolder()
        {
            var cacheFolder = string.Format(CacheFolder);
            if (!await _storage.Local.DirectoryExistsAsync(cacheFolder))
            {
                await _storage.Local.CreateDirectoryAsync(cacheFolder);
            }
        }

        private async Task CheckAndCreateSinceFile()
        {
            var filename = GetSinceFileName();
            if (await _storage.Local.FileExistsAsync(filename))
            {
                var json = await _storage.Local.ReadAllTextAsync(filename);
                _cacheSince = await json.DeserialiseAsync<CacheSince>();
            }
            else
            {
                _cacheSince = new CacheSince();
            }
        }

        private async Task WriteSinceToStorage()
        {
            var filename = GetSinceFileName();
            var json = await _cacheSince.SerialiseAsync();
            await _storage.Local.WriteAllTextAsync(filename, json);
        }

        private async Task WriteCountsToStorage()
        {
            var filename = GetFileName(CountsFile);
            var json = await CacheCount.SerialiseAsync();
            await _storage.Local.WriteAllTextAsync(filename, json);
        }

        private async Task ReadCountsFromStorage()
        {
            var filename = GetFileName(CountsFile);
            if (await _storage.Local.FileExistsAsync(filename))
            {
                var json = await _storage.Local.ReadAllTextAsync(filename);
                CacheCount = await json.DeserialiseAsync<CacheCount>();
            }
            else
            {
                CacheCount = new CacheCount();
            }
        }

        private bool CacheHasExpired(SourceProvider type)
        {
            DateTime? date;
            switch (type)
            {
                case SourceProvider.Pocket:
                    date = _cacheSince.PocketSince;
                    break;
                case SourceProvider.Instapaper:
                    date = _cacheSince.InstapaperSince;
                    break;
                case SourceProvider.Readability:
                    date = _cacheSince.ReadabilitySince;
                    break;
                default:
                    return true;
            }

            var expired = true;
            if (date.HasValue)
            {
                var difference = DateTime.Now - date.Value;
                expired = difference.TotalMinutes > 15;
            }

            return expired;
        }
    }
}