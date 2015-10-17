using System.Collections.Generic;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;

namespace Speader.Design
{
    public class MessageBoxServiceDesign : IMessageBoxService
    {
        public Task ShowAsync(string text)
        {
            return null;
        }

        public Task ShowAsync(string text, string caption)
        {
            return null;
        }

        public async Task<int> ShowAsync(string text, string caption, IEnumerable<string> buttons)
        {
            return 0;
        }
    }
}
