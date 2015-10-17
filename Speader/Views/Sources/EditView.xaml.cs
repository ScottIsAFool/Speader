using Windows.UI.Xaml;

namespace Speader.Views.Sources
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditView
    {
        public EditView()
        {
            InitializeComponent();
            Loaded += (sender, args) => EdiTextBox.Focus(FocusState.Keyboard);
        }
    }
}
