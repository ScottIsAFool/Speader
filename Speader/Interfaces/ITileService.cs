using System.Threading.Tasks;
using Speader.Model;

namespace Speader.Interfaces
{
    public interface ITileService
    {
        Task SetTileTransparency();
        Task<bool> PinSource(SourceProvider source);
        Task<bool> UnpinSource(SourceProvider source);
        bool IsLocalPinned { get; }
        bool IsPocketPinned { get; }
        bool IsReadabilityPinned { get; }
        bool IsInstapaperPinned { get; }
    }
}
