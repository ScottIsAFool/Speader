using System;
using PocketSharp.Models;
using Readability.Models;
using Speader.Model;
using Bookmark = Insta.Portable.Models.Bookmark;

namespace Speader.Extensions
{
    public static class ReaderItemExtensions
    {
        public static ReaderItem ToReaderItem(this PocketArticle article)
        {
            var result = new ReaderItem
            {
                Id = article.ID ?? string.Empty,
                Source= SourceProvider.Pocket,
                Author = article.Authors == null ? string.Empty : string.Join(", ", article.Authors) ?? string.Empty,
                Title = article.Title ?? string.Empty,
                Url = article.Uri.AbsoluteUri ?? string.Empty,
                Excerpt = string.Empty,
                Text = article.Content.StripHtmlTags() ?? string.Empty,
                CreatedDate = article.PublishedTime ?? DateTime.Now
            };

            result.WordCount = string.IsNullOrEmpty(result.Text) ? 0 : result.Text.Split(new[] {' '}).Length;
            return result;
        }

        public static ReaderItem ToReaderItem(this ExtendedPocketItem article)
        {
            var result = new ReaderItem
            {
                Id = article.ResolvedId ?? string.Empty,
                Source = SourceProvider.Pocket,
                Author = article.Authors == null ? string.Empty : string.Join(", ", article.Authors) ?? string.Empty,
                Title = article.DisplayTitle ?? string.Empty,
                Text = string.Empty,
                Url = article.Uri.ToString() ?? string.Empty,
                Excerpt = article.Excerpt ?? string.Empty,
                CreatedDate = article.UpdateTime ?? DateTime.Now
            };

            result.WordCount = string.IsNullOrEmpty(result.Text) ? 0 : result.Text.Split(new[] { ' ' }).Length;
            return result;
        }

        public static ReaderItem ToReaderItem(this Article article)
        {
            var result = new ReaderItem
            {
                Id = article.Id ?? string.Empty,
                Source = SourceProvider.Readability,
                Author = article.Author ?? string.Empty,
                Title = article.Title ?? string.Empty,
                Url = article.Url ?? string.Empty,
                Excerpt = article.Excerpt ?? string.Empty,
                WordCount = article.WordCount,
                CreatedDate = article.DatePublished ?? DateTime.Now,
                Text = string.IsNullOrEmpty(article.Content) ? string.Empty : article.Content.StripHtmlTags()
            };

            return result;
        }

        public static ReaderItem ToReaderItem(this Bookmark article)
        {
            var result = new ReaderItem
            {
                Id = article.Id.ToString(),
                Source = SourceProvider.Instapaper,
                Author = string.Empty,
                Title = article.Title ?? string.Empty,
                Url = article.Url ?? string.Empty,
                Excerpt = article.Description ?? string.Empty,
                Text= string.Empty,
                CreatedDate = article.Time ?? DateTime.Now
            };

            return result;
        }
    }
}