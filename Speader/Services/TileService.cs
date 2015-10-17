using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Cimbalino.Toolkit.Extensions;
using Cimbalino.Toolkit.Services;
using NotificationsExtensions.TileContent;
using ScottIsAFool.Windows.Core.Extensions;
using ScottIsAFool.WindowsPhone.Logging;
using Speader.Extensions;
using Speader.Interfaces;
using Speader.Model;

namespace Speader.Services
{
    public class TileService : ITileService
    {
        private readonly IStorageService _storageService;
        private readonly ISettingsService _settingsService;
        private readonly ILog _logger;

        private const string MediumTileLocation = "ms-appdata:///Assets/{0}Square150x150.png";
        private const string WideTileLocation = "ms-appdata:///Assets/{0}Wide310x150.png";
        private const string SourceTileLocation = "ms-appdata:///Local/" + SourceTileFile;
        private const string SourceTileFile = "{0}{1}TileBackground.png";
        private const string Arguments = "http://speaderapp.com/?source={0}";
        private const string Transparent = "Transparent/";

        public TileService(
            IStorageService storageService,
            ISettingsService settingsService)
        {
            _storageService = storageService;
            _settingsService = settingsService;
            _logger = new WinLogger("TileService");
        }

        public async Task SetTileTransparency()
        {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            if (updater == null)
            {
                return;
            }
            updater.EnableNotificationQueue(true);
            updater.Clear();

            var tileType = _settingsService.UseTransparentTile ? Transparent : string.Empty;

            var mainWideContent = TileContentFactory.CreateTileWide310x150ImageAndText02();
            mainWideContent.Image.Src = string.Format(WideTileLocation, tileType);
            mainWideContent.Branding = TileBranding.None;

            var mainSquareContent = CreateMainTile(string.Format(MediumTileLocation, tileType), TileBranding.None);

            mainWideContent.Square150x150Content = mainSquareContent;
            updater.Update(mainWideContent.CreateNotification());

            var secondaryTiles = await SecondaryTile.FindAllAsync();
            foreach(var tile in secondaryTiles)
            {
                var arguments = new Uri(tile.Arguments).QueryDictionary();
                var source = arguments["source"];
                var uri = GetTileUrl(source);
                updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId);

                mainSquareContent = CreateMainTile(uri, TileBranding.Name);

                updater.Update(mainSquareContent.CreateNotification());
            }
        }

        private static ITileSquare150x150PeekImageAndText04 CreateMainTile(string uri, TileBranding branding)
        {
            var mainSquareContent = TileContentFactory.CreateTileSquare150x150PeekImageAndText04();
            mainSquareContent.Image.Src = uri;
            mainSquareContent.Branding = branding;

            var smallSquare = TileContentFactory.CreateTileSquare71x71IconWithBadge();
            smallSquare.ImageIcon.Src = uri;

            mainSquareContent.Square71x71Content = smallSquare;
            return mainSquareContent;
        }

        public async Task<bool> PinSource(SourceProvider source)
        {
            var id = ToString(source);
            if (string.IsNullOrEmpty(id)) return false;

            var uri = GetTileUrl(source.ToString());

            //var file = GetTileFile(source.ToString(), _settingsService.UseTransparentTile);
            //var tile = new SourceTile();
            //tile.SetTile(_settingsService.UseTransparentTile, source);

            //await SaveVisualElementToFile(tile, file, 360, 360);
            
            var arguments = string.Format(Arguments, source);

            try
            {
                var secondaryTile = new SecondaryTile(id, source.ToLocalisedSource(), arguments, new Uri(uri, UriKind.Absolute), TileSize.Square150x150) {BackgroundColor = Colors.Transparent};
                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
                secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark;
                secondaryTile.VisualElements.Square30x30Logo = new Uri(uri);

                return await secondaryTile.RequestCreateAsync();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("PinSource(" + source + ")", ex);
                return false;
            }
        }

        private string GetTileUrl(string source)
        {
            var tileType = _settingsService.UseTransparentTile ? Transparent : string.Empty;
            return string.Format(SourceTileLocation, tileType, source);
        }

        public string GetTileFile(string source, bool isTransparent)
        {
            var tileType = isTransparent ? Transparent : string.Empty;
            return string.Format(SourceTileFile, tileType, source);
        }

        public async Task<bool> UnpinSource(SourceProvider source)
        {
            var id = ToString(source);
            if (string.IsNullOrEmpty(id)) return false;

            var tiles = await SecondaryTile.FindAllAsync();
            var tile = tiles.FirstOrDefault(x => x.TileId == id);
            if (tile == null) return false;

            return await tile.RequestDeleteAsync();
        }

        public bool IsLocalPinned => IsPinned(Constants.Tiles.LocalId);

        public bool IsPocketPinned => IsPinned(Constants.Tiles.PocketId);

        public bool IsReadabilityPinned => IsPinned(Constants.Tiles.ReadabilityId);

        public bool IsInstapaperPinned => IsPinned(Constants.Tiles.InstapaperId);

        private static bool IsPinned(string tileId)
        {
            return SecondaryTile.Exists(tileId);
        }

        private static string ToString(SourceProvider source)
        {
            switch (source)
            {
                case SourceProvider.Local:
                    return Constants.Tiles.LocalId;
                case SourceProvider.Instapaper:
                    return Constants.Tiles.InstapaperId;
                case SourceProvider.Readability:
                    return Constants.Tiles.ReadabilityId;
                case SourceProvider.Pocket:
                    return Constants.Tiles.PocketId;
                default:
                    return string.Empty;
            }
        }

        public async Task SaveVisualElementToFile(UIElement element, string filename, int height, int width)
        {
            //element.Measure(new Size(width, height));
            //element.Arrange(new Rect { Height = height, Width = width });
            //element.UpdateLayout();
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(element, width, height);

            using (var file = await _storageService.Local.CreateFileAsync(filename))
            {
                await renderTargetBitmap.SavePngAsync(file);
            }
        }
    }
}