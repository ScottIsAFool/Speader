using System.Threading.Tasks;
using Speader.Interfaces;
using Speader.Model;

namespace Speader.Design
{
    public class TileServiceDesign : ITileService
    {
        public async Task SetTileTransparency()
        {
        }

        public async Task<bool> PinSource(SourceProvider source)
        {
            return false;
        }

        public async Task<bool> UnpinSource(SourceProvider source)
        {
            return false;
        }

        public bool IsLocalPinned { get; private set; }
        public bool IsPocketPinned { get; private set; }
        public bool IsReadabilityPinned { get; private set; }
        public bool IsInstapaperPinned { get; private set; }
    }
}