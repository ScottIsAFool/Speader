using System;
using System.Threading.Tasks;
using Speader.Interfaces;
using Speader.Messages;
using Speader.Model;

namespace Speader.Design
{
    public class ReaderHelperDesign : IReaderHelper
    {
        public Task<ReaderItem> HandleProtocolMessage(UriSchemeMessage m)
        {
            return null;
        }

        public Task<ReaderItem> SaveArticle(Uri url)
        {
            return null;
        }

        public async Task SaveEditedArticle(ReaderItem copyItem)
        {
        }
    }
}