using System;
using System.Threading.Tasks;
using Speader.Messages;
using Speader.Model;

namespace Speader.Interfaces
{
    public interface IReaderHelper
    {
        Task<ReaderItem> HandleProtocolMessage(UriSchemeMessage m);
        Task<ReaderItem> SaveArticle(Uri url);
        Task SaveEditedArticle(ReaderItem copyItem);
    }
}