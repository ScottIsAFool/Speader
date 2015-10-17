using System;
using System.Linq;
using System.Threading.Tasks;
using PocketSharp;
using Speader.Extensions;
using Speader.Interfaces;
using Speader.Messages;
using Speader.Model;

namespace Speader.Helpers
{
    public class ReaderHelper : IReaderHelper
    {
        private readonly IPocketClient _pocketClient;
        private readonly ICacheService _cacheService;

        public ReaderHelper(IPocketClient pocketClient, ICacheService cacheService)
        {
            _pocketClient = pocketClient;
            _cacheService = cacheService;
        }

        public async Task<ReaderItem> SaveArticle(Uri url)
        {
            var article = await _pocketClient.GetArticle(url, false, false);
            if (article != null)
            {
                var savedArticle = article.ToReaderItem();
                savedArticle.Source = SourceProvider.Local;

                var response = await _cacheService.GetLocalItemsFromCache();
                var articleExists = response.ReaderItems.FirstOrDefault(x => x.Id == savedArticle.Id);
                if (articleExists != null)
                {
                    response.ReaderItems.Remove(articleExists);
                }

                await _cacheService.SaveArticle(savedArticle);

                savedArticle.Text = string.Empty;

                response.ReaderItems.Insert(0, savedArticle);

                await _cacheService.SaveLocalItems(response.ReaderItems);

                return savedArticle;
            }

            return null;
        }

        public async Task SaveEditedArticle(ReaderItem copyItem)
        {
            var response = await _cacheService.GetLocalItemsFromCache();
            var isAdd = false;

            var existingItem = response.ReaderItems.FirstOrDefault(x => x.Id == copyItem.Id);
            if (existingItem == null)
            {
                isAdd = true;
                copyItem.Text = copyItem.Text.Clean();
                var title = copyItem.Text.Length > 150 ? copyItem.Text.Substring(0, 150).Trim() : copyItem.Text;
                copyItem.Title = title;
                copyItem.CreatedDate = DateTime.Now;
                copyItem.Url = copyItem.Author = string.Empty;

                existingItem = copyItem;
            }
            else
            {
                existingItem.Text = copyItem.Text.Clean();
            }

            existingItem.Excerpt = copyItem.Text.ToExcerpt();
            existingItem.WordCount = copyItem.Text.Split(' ').Length;

            await _cacheService.SaveArticle(existingItem);

            existingItem.Text = string.Empty;

            if (isAdd)
            {
                response.ReaderItems.Insert(0, existingItem);
            }

            await _cacheService.SaveLocalItems(response.ReaderItems);
        }

        public async Task<ReaderItem> HandleProtocolMessage(UriSchemeMessage m)
        {
            if (m.IsUri)
            {
                var item = await SaveArticle(new Uri(m.Content));
                if (item != null)
                {
                    var fullItem = await _cacheService.GetArticle(item);
                    return fullItem;
                }
            }
            else
            {
                var item = ReaderItem.NewLocalItem(m.Content);
                await SaveEditedArticle(item);
                return item;
            }

            return null;
        }
    }
}
