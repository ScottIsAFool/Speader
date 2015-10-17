using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Speader.Collections
{
    public class PaginatedCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        private readonly Func<uint, Task<IList<T>>> _load;

        public PaginatedCollection(Func<uint, Task<IList<T>>> load)
        {
            _load = load;
            HasMoreItems = true;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run(async c =>
            {
                var data = await _load(count);

                foreach (var item in data)
                    Add(item);

                HasMoreItems = data.Any();

                return new LoadMoreItemsResult
                {
                    Count = (uint) data.Count
                };
            });
        }

        public bool HasMoreItems { get; private set; }
    }
}
