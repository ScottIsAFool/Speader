using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Speader.Controls
{
    public class ReaderItemControl : Control
    {
        private Grid _grid;

        public static readonly DependencyProperty ArchiveCommandProperty = DependencyProperty.Register(
            "ArchiveCommand", typeof (ICommand), typeof (ReaderItemControl), new PropertyMetadata(default(ICommand)));

        public ICommand ArchiveCommand
        {
            get { return (ICommand) GetValue(ArchiveCommandProperty); }
            set { SetValue(ArchiveCommandProperty, value); }
        }

        public static readonly DependencyProperty ReaderCommandProperty = DependencyProperty.Register(
            "ReaderCommand", typeof (ICommand), typeof (ReaderItemControl), new PropertyMetadata(default(ICommand)));

        public ICommand ReaderCommand
        {
            get { return (ICommand) GetValue(ReaderCommandProperty); }
            set { SetValue(ReaderCommandProperty, value); }
        }

        public static readonly DependencyProperty FullPageCommandProperty = DependencyProperty.Register(
            "FullPageCommand", typeof (ICommand), typeof (ReaderItemControl), new PropertyMetadata(default(ICommand)));

        public ICommand FullPageCommand
        {
            get { return (ICommand) GetValue(FullPageCommandProperty); }
            set { SetValue(FullPageCommandProperty, value); }
        }

        public static readonly DependencyProperty IsDeleteProperty = DependencyProperty.Register(
            "IsDelete", typeof (bool), typeof (ReaderItemControl), new PropertyMetadata(default(bool)));

        public bool IsDelete
        {
            get { return (bool) GetValue(IsDeleteProperty); }
            set { SetValue(IsDeleteProperty, value); }
        }

        public static readonly DependencyProperty IsArchiveProperty = DependencyProperty.Register(
            "IsArchive", typeof (bool), typeof (ReaderItemControl), new PropertyMetadata(default(bool)));

        public bool IsArchive
        {
            get { return (bool) GetValue(IsArchiveProperty); }
            set { SetValue(IsArchiveProperty, value); }
        }

        public static readonly DependencyProperty IsRemoveProperty = DependencyProperty.Register(
            "IsRemove", typeof (bool), typeof (ReaderItemControl), new PropertyMetadata(default(bool)));

        public bool IsRemove
        {
            get { return (bool) GetValue(IsRemoveProperty); }
            set { SetValue(IsRemoveProperty, value); }
        }

        public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(
            "Percentage", typeof (double), typeof (ReaderItemControl), new PropertyMetadata(default(double)));

        public double Percentage
        {
            get { return (double) GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        public static readonly DependencyProperty DisplayContextMenusProperty = DependencyProperty.Register(
            "DisplayContextMenus", typeof (bool), typeof (ReaderItemControl), new PropertyMetadata(true));

        public bool DisplayContextMenus
        {
            get { return (bool) GetValue(DisplayContextMenusProperty); }
            set { SetValue(DisplayContextMenusProperty, value); }
        }

        public ReaderItemControl()
        {
            DefaultStyleKey = typeof (ReaderItemControl);
        }

        protected override void OnApplyTemplate()
        {
            _grid = GetTemplateChild("LayoutGrid") as Grid;

            if (_grid != null)
            {
                _grid.Holding -= GridOnHolding;
                _grid.Holding += GridOnHolding;
            }

            base.OnApplyTemplate();
        }

        private void GridOnHolding(object sender, HoldingRoutedEventArgs holdingRoutedEventArgs)
        {
            if (DisplayContextMenus)
            {
                FlyoutBase.ShowAttachedFlyout(_grid);
            }
        }
    }
}
