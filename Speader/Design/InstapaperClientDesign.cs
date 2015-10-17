using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AsyncOAuth;
using Insta.Portable;
using Insta.Portable.Models;

namespace Speader.Design
{
    public class InstapaperClientDesign : IInstapaperClient
    {
        public Task<AccessToken> GetAuthTokenAsync(string emailAddress, string password, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<InstaResponse<User>> VerifyUserAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<InstaResponse<BookmarksResponse>> GetBookmarksAsync(int? limit = null, string folderType = null, IEnumerable<int> alreadyHave = null, IEnumerable<int> highlights = null, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<string> GetBookmarkContentAsync(int bookmarkId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<InstaResponse<Bookmark>> AddBookmarkAsync(string bookmarkUrl, string title = null, string description = null, string folderId = null, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<InstaResponse<Bookmark>> UpdateReadProgressAsync(int bookmarkId, float readPercentage, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteBookmarkAsync(int bookmarkId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<InstaResponse<Bookmark>> StarBookmarkAsync(int bookmarkId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<InstaResponse<Bookmark>> UnstarBookmarkAsync(int bookmarkId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<InstaResponse<Bookmark>> ArchiveBookmarkAsync(int bookmarkId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<InstaResponse<Bookmark>> UnarchiveBookmarkAsync(int bookmarkId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<InstaResponse<IEnumerable<Folder>>> GetFoldersAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<InstaResponse<Folder>> AddFolderAsync(string title, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFolderAsync(string folderId, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public AccessToken AccessToken { get; set; }
    }
}
