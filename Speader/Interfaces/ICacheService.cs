using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Speader.Model;

namespace Speader.Interfaces
{
    public interface ICacheService
    {
        CacheCount CacheCount { get; set; }
        Task StartService();
        Task<CacheResponse> GetPocketItemsFromCache();
        Task SavePocketItems(ObservableCollection<ReaderItem> items);
        Task ClearPocketItems();
        Task<CacheResponse> GetReadabilityItemsFromCache();
        Task SaveReadabilityItems(ObservableCollection<ReaderItem> items);
        Task ClearReadabilityItems();
        Task<CacheResponse> GetLocalItemsFromCache();
        Task SaveLocalItems(IList<ReaderItem> items);
        Task ClearLocalItems();
        Task<CacheResponse> GetInstapaperItemsFromCache();
        Task SaveInstapaperItems(ObservableCollection<ReaderItem> items);
        Task ClearInstapaperItems();
        Task<ReaderItem> GetArticle(ReaderItem item);
        Task<CacheResponse> GetInProgressItemsFromCache();
        Task SaveInProgressItems(IList<ReaderItem> items);
        Task ClearInProgressItems();
        Task<CacheResponse> GetRecentItemsFromCache();
        Task SaveRecentItems(IList<ReaderItem> items);
        Task ClearRecentItems();
        Task SaveArticle(ReaderItem item);
        Task DeleteArticle(ReaderItem item);
        Task ClearAllItems();
        Task InvalidateCache();
        DateTime? GetSinceDate(SourceProvider type);
        Task ClearAllSinceDates();
        CacheCount GetCounts();
    }
}