using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Speader.Controls
{
    public class SavingOverlay : Control
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof (string), typeof (SavingOverlay), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ProgressIsActiveProperty = DependencyProperty.Register(
            "ProgressIsActive", typeof (bool), typeof (SavingOverlay), new PropertyMetadata(default(bool)));

        public bool ProgressIsActive
        {
            get { return (bool) GetValue(ProgressIsActiveProperty); }
            set { SetValue(ProgressIsActiveProperty, value); }
        }

        public SavingOverlay()
        {
            DefaultStyleKey = typeof (SavingOverlay);
        }
    }
}
