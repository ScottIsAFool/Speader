using PocketSharp.Models;
using Speader.Model;

namespace Speader.Extensions
{
    public static class PocketExtensions
    {
        public static ExtendedPocketItem Extend(this PocketItem item)
        {
            var extendedItem = new ExtendedPocketItem();
            item.CopyItem(extendedItem);
            //var json = JsonConvert.SerializeObject(item);

            //var extended = JsonConvert.DeserializeObject<ExtendedPocketItem>(json);

            ////extended.Excerpt = string.Empty;

            return extendedItem;
        }
    }
}
