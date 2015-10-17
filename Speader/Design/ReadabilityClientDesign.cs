using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncOAuth;
using Readability;
using Readability.Models;

namespace Speader.Design
{
    public class ReadabilityClientDesign : IReadabilityClient
    {
        public Task<string> GetRequestTokenAsync(string callbackUri, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Uri GenerateAuthenticationUri()
        {
            throw new NotImplementedException();
        }

        public Task<AccessToken> VerifyUserAsync(string verifier)
        {
            throw new NotImplementedException();
        }

        public Task<UserProfile> GetProfileAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Article> GetArticleAsync(string articleId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddBookmarkAsync(string url, bool favorite = false, bool archive = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteBookmarkAsync(int bookmarkId)
        {
            throw new NotImplementedException();
        }

        public Task<BookmarksResponse> GetBookmarksAsync(Conditions conditions)
        {
            throw new NotImplementedException();
        }

        public Task<Bookmark> GetBookmarkAsync(int bookmarkId)
        {
            throw new NotImplementedException();
        }

        public Task<Bookmark> SetBookmarkArchiveStateAsync(int bookmarkId, bool isArchived)
        {
            throw new NotImplementedException();
        }

        public Task<Bookmark> SetBookmarkFavoriteStateAsync(int bookmarkId, bool isFavorite)
        {
            throw new NotImplementedException();
        }

        public Task<Bookmark> SetBookmarkReadPercentageAsync(int bookmarkId, float readPercentage)
        {
            throw new NotImplementedException();
        }

        public AccessToken AccessToken { get; set; }
    }
}
