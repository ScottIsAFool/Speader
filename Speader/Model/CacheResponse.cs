using System.Collections.Generic;
using System.Diagnostics;

namespace Speader.Model
{
    [DebuggerDisplay("Items count: {ReaderItems.Count}, CacheExpired: {CacheExpired}")]
    public class CacheResponse
    {
        public CacheResponse(IList<ReaderItem> readerItems, bool cacheExpired)
        {
            ReaderItems = readerItems;
            CacheExpired = cacheExpired;
        }

        public IList<ReaderItem> ReaderItems { get; set; }
        public bool CacheExpired { get; set; }
    }
}
