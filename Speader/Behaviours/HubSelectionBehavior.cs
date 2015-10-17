using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Cimbalino.Toolkit.Behaviors;
using Speader.Extensions;

namespace Speader.Behaviours
{
    public class HubSelectionBehavior : Behavior<Hub>
    {
        private ScrollViewer _scroller;
        private bool _settingIndex;

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
            "SelectedIndex",
            typeof(int),
            typeof(HubSelectionBehavior),
            new PropertyMetadata(0, OnSelectedIndexChanged));

        protected override void OnAttached()
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            var hub = AssociatedObject;
            if (null == hub) return;

            _scroller = hub.GetChildOfType<ScrollViewer>();
            if (_scroller == null)
            {
                hub.Loaded += OnHubLoaded;
            }
            else
            {
                _scroller.ViewChanged += ScrollerOnViewChanged;
            }
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            if (_scroller != null)
            {
                _scroller.ViewChanged -= ScrollerOnViewChanged;
            }
            base.OnDetaching();
        }

        private void OnHubLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var hub = (Hub)sender;

            _scroller = hub.GetChildOfType<ScrollViewer>();
            if (_scroller != null)
            {
                _scroller.ViewChanged += ScrollerOnViewChanged;
                hub.Loaded -= OnHubLoaded;
            }
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled) return;

            var behavior = d as HubSelectionBehavior;
            if (behavior == null) return;
            if (behavior._settingIndex) return;

            var hub = behavior.AssociatedObject;
            if (hub == null) return;
            if (hub.Sections.Count == 0) return;

            if (behavior.SelectedIndex < 0) return;

            var section = hub.Sections[behavior.SelectedIndex];
            hub.ScrollToSection(section);
        }

        private void ScrollerOnViewChanged(object sender, ScrollViewerViewChangedEventArgs scrollViewerViewChangedEventArgs)
        {
            if (DesignMode.DesignModeEnabled) return;

            var hub = AssociatedObject;

            _settingIndex = true;
            SelectedIndex = hub.Sections.IndexOf(hub.SectionsInView[0]);
            _settingIndex = false;
        }
    }
}