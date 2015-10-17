using System;
using System.IO;
using PropertyChanged;
using Speader.Extensions;
using Speader.Interfaces;

namespace Speader.Model
{
    [ImplementPropertyChanged]
    public class ReaderItem : IBinarySerializable
    {
        public string Id { get; set; }

        public SourceProvider Source { get; set; }
        public bool IsDownloaded { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public string Excerpt { get; set; }
        
        public int WordCount { get; set; }

        public int WordsRead { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Url { get; set; }

        public string DisplayExcerpt
        {
            get
            {
                if (!string.IsNullOrEmpty(Excerpt))
                {
                    return Excerpt.Clean();
                }

                return string.IsNullOrEmpty(Text) ? CreatedDate.ToString() : Text.Clean();
            }
        }

        internal string InternalId => $"{Source}.{Id}";

        public double PercentageRead
        {
            get
            {
                var percent = (WordsRead/(double) WordCount);
                return percent > 1 ? 1 : percent;
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write((int)Source);
            writer.Write(IsDownloaded);
            writer.Write(Title);
            writer.Write(Author);
            writer.Write(Excerpt);
            writer.Write(Text);
            writer.Write(WordCount);
            writer.Write(WordsRead);
            writer.Write(CreatedDate);
            writer.Write(Url);
        }

        public T InitFromCache<T>(BinaryReader reader) where T : class, IBinarySerializable
        {
            var item = new ReaderItem();
            item.Read(reader);
            return item as T;
        }

        public void Read(BinaryReader reader)
        {
            Id = reader.ReadString();
            Source = (SourceProvider)reader.ReadInt32();
            IsDownloaded = reader.ReadBoolean();
            Title = reader.ReadString();
            Author = reader.ReadString();
            Excerpt = reader.ReadString();
            Text = reader.ReadString();
            WordCount = reader.ReadInt32();
            WordsRead = reader.ReadInt32();
            CreatedDate = reader.ReadDateTime();
            Url = reader.ReadString();
        }

        public static ReaderItem NewLocalItem(string content = "")
        {
            var result = new ReaderItem
            {
                Source = SourceProvider.Local,
                Id = Guid.NewGuid().ToString(),
                Text = content
            };
            return result;
        }
    }
}