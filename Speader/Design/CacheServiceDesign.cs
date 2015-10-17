using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Speader.Interfaces;
using Speader.Model;

namespace Speader.Design
{
    public class CacheServiceDesign : ICacheService
    {
        public CacheCount CacheCount { get; set; }

        public Task StartService()
        {
            throw new NotImplementedException();
        }

        public Task<CacheResponse> GetPocketItemsFromCache()
        {
            throw new NotImplementedException();
        }

        public Task SavePocketItems(ObservableCollection<ReaderItem> items)
        {
            throw new NotImplementedException();
        }

        public Task ClearPocketItems()
        {
            throw new NotImplementedException();
        }

        public Task<CacheResponse> GetReadabilityItemsFromCache()
        {
            throw new NotImplementedException();
        }

        public Task SaveReadabilityItems(ObservableCollection<ReaderItem> items)
        {
            throw new NotImplementedException();
        }

        public Task ClearReadabilityItems()
        {
            throw new NotImplementedException();
        }

        public Task<CacheResponse> GetLocalItemsFromCache()
        {
            throw new NotImplementedException();
        }

        public Task SaveLocalItems(IList<ReaderItem> items)
        {
            throw new NotImplementedException();
        }

        public Task ClearLocalItems()
        {
            throw new NotImplementedException();
        }

        public Task<CacheResponse> GetInstapaperItemsFromCache()
        {
            throw new NotImplementedException();
        }

        public Task SaveInstapaperItems(ObservableCollection<ReaderItem> items)
        {
            throw new NotImplementedException();
        }

        public Task ClearInstapaperItems()
        {
            throw new NotImplementedException();
        }

        public Task<ReaderItem> GetArticle(ReaderItem item)
        {
            throw new NotImplementedException();
        }

        public Task<CacheResponse> GetInProgressItemsFromCache()
        {
            throw new NotImplementedException();
        }

        public Task SaveInProgressItems(IList<ReaderItem> items)
        {
            throw new NotImplementedException();
        }

        public Task ClearInProgressItems()
        {
            throw new NotImplementedException();
        }

        public Task<CacheResponse> GetRecentItemsFromCache()
        {
            throw new NotImplementedException();
        }

        public Task SaveRecentItems(IList<ReaderItem> items)
        {
            throw new NotImplementedException();
        }

        public Task ClearRecentItems()
        {
            throw new NotImplementedException();
        }

        public Task SaveArticle(ReaderItem item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteArticle(ReaderItem item)
        {
            throw new NotImplementedException();
        }

        public Task ClearAllItems()
        {
            throw new NotImplementedException();
        }

        public Task InvalidateCache()
        {
            throw new NotImplementedException();
        }

        public DateTime? GetSinceDate(SourceProvider type)
        {
            throw new NotImplementedException();
        }

        public Task ClearAllSinceDates()
        {
            throw new NotImplementedException();
        }

        public CacheCount GetCounts()
        {
            throw new NotImplementedException();
        }
    }
}