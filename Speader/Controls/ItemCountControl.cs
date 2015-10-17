using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Speader.Controls
{
    public class ItemCountControl : Control
    {
        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(
            "Count", typeof (object), typeof (ItemCountControl), new PropertyMetadata(null, OnCountChanged));

        public object Count
        {
            get { return GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            "DisplayText", typeof (string), typeof (ItemCountControl), new PropertyMetadata(default(string)));

        public string DisplayText
        {
            get { return (string) GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        private static void OnCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var icc = sender as ItemCountControl;

            icc?.SetStates();
        }

        public ItemCountControl()
        {
            DefaultStyleKey = typeof (ItemCountControl);
        }

        protected override void OnApplyTemplate()
        {
            SetStates();
            base.OnApplyTemplate();
        }

        private void SetStates()
        {
            var count = (int?)Count;
            DisplayText = count.HasValue ? count.Value.ToString() : string.Empty;
            Visibility = count.HasValue && count.Value > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
