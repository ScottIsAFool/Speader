using GalaSoft.MvvmLight.Messaging;
using Speader.Controls;
using Speader.Model;

namespace Speader.Messages
{
    public class TileMessage : MessageBase
    {
        public TileMessage(SourceTile tile, SourceProvider provider)
        {
            Tile = tile;
            SourceProvider = provider;
        }

        public SourceTile Tile { get; set; }
        public SourceProvider SourceProvider { get; set; }
    }
}