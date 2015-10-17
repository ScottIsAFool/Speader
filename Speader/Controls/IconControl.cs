using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Speader.Controls
{
    public class IconControl : Control
    {
        public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register(
            "IconData", typeof (string), typeof (IconControl), new PropertyMetadata(default(string)));

        public string IconData
        {
            get { return (string) GetValue(IconDataProperty); }
            set { SetValue(IconDataProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof (string), typeof (IconControl), new PropertyMetadata(default(string)));

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill", typeof (SolidColorBrush), typeof (IconControl), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush Fill
        {
            get { return (SolidColorBrush) GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public IconControl()
        {
            DefaultStyleKey = typeof (IconControl);
        }
    }
}
