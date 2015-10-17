using PocketSharp.Models;
using PropertyChanged;

namespace Speader.Model
{
    [ImplementPropertyChanged]
    public class ExtendedPocketItem : PocketItem
    {
        public string DisplayDate => UpdateTime.HasValue ? UpdateTime.Value.ToString("D") : string.Empty;

        public string DisplayTitle => string.IsNullOrEmpty(FullTitle) ? Title : FullTitle;
    }
}
