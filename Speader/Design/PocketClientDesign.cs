using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PocketSharp;
using PocketSharp.Models;

namespace Speader.Design
{
    public class PocketClientDesign : IPocketClient
    {
        public Task<string> GetRequestCode(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Uri GenerateAuthenticationUri(string requestCode = null)
        {
            throw new NotImplementedException();
        }

        public Task<PocketUser> GetUser(string requestCode = null, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Uri GenerateRegistrationUri(string requestCode = null)
        {
            throw new NotImplementedException();
        }

        public Task<PocketItem> Add(Uri uri, string[] tags = null, string title = null, string tweetID = null, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PocketItem>> Get(State? state = null, bool? favorite = null, string tag = null, ContentType? contentType = null, Sort? sort = null, string search = null, string domain = null, DateTime? since = null, int? count = null, int? offset = null, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<PocketItem> Get(string itemID, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PocketItem>> Get(RetrieveFilter filter, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PocketItem> ConvertJsonToList(string itemsJSON)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PocketTag>> GetTags(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PocketItem>> SearchByTag(string tag, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PocketItem>> Search(string searchString, string tag = null, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<PocketSharp.Models.PocketArticle> GetArticle(Uri uri, bool includeImages = true, bool includeVideos = true, bool forceRefresh = false, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendActions(IEnumerable<PocketAction> actions, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> Archive(string itemID, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> Archive(PocketItem item, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> Unarchive(string itemID, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> Unarchive(PocketItem item, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> Favorite(string itemID, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> Favorite(PocketItem item, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> Unfavorite(string itemID, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> Unfavorite(PocketItem item, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string itemID, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(PocketItem item, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddTags(string itemID, string[] tags, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddTags(PocketItem item, string[] tags, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveTags(string itemID, string[] tags, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveTags(PocketItem item, string[] tags, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveTag(string itemID, string tag, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveTag(PocketItem item, string tag, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveTags(string itemID, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveTags(PocketItem item, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceTags(string itemID, string[] tags, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceTags(PocketItem item, string[] tags, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> RenameTag(string itemID, string oldTag, string newTag, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> RenameTag(PocketItem item, string oldTag, string newTag, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<PocketStatistics> GetUserStatistics(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<PocketLimits> GetUsageLimits(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string CallbackUri { get; set; }
        public string ConsumerKey { get; set; }
        public string RequestCode { get; set; }
        public string AccessCode { get; set; }
        public Action<string> PreRequest { get; set; }
    }
}
