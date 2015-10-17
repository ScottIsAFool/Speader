using Windows.UI.Xaml;
using Speader.Model;

namespace Speader.Controls
{
    public sealed partial class SourceTile
    {
        public SourceTile()
        {
            InitializeComponent();
        }

        public void SetTile(bool isTransparent, SourceProvider source)
        {
            NormalBackground.Visibility = isTransparent ? Visibility.Collapsed : Visibility.Visible;
            TransparentBackrgound.Visibility = isTransparent ? Visibility.Visible : Visibility.Collapsed;

            switch (source)
            {
                case SourceProvider.Instapaper:
                    InstapaperIcon.Visibility = Visibility.Visible;
                    break;
                case SourceProvider.Pocket:
                    PocketIcon.Visibility = Visibility.Visible;
                    break;
                case SourceProvider.Readability:
                    ReadabilityIcon.Visibility = Visibility.Visible;
                    break;
                case SourceProvider.Local:
                    LocalIcon.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
