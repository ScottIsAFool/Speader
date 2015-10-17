using PropertyChanged;

namespace Speader.Model
{
    [ImplementPropertyChanged]
    public class CacheCount
    {
        public int? PocketCount { get; set; }
        public int? ReadabilityCount { get; set; }
        public int? InstapaperCount { get; set; }
        public int? LocalCount { get; set; }
    }
}