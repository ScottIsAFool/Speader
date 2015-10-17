using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Cimbalino.Toolkit.Behaviors;

namespace Speader.Behaviours
{
    public class WebViewBehaviour : Behavior<WebView>
    {
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
            "IsLoading", typeof (bool), typeof (WebViewBehaviour), new PropertyMetadata(default(bool)));

        public bool IsLoading
        {
            get { return (bool) GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly DependencyProperty ActualUriProperty = DependencyProperty.Register(
            "ActualUri", typeof (Uri), typeof (WebViewBehaviour), new PropertyMetadata(default(Uri), OnActualUriChanged));

        public Uri ActualUri
        {
            get { return (Uri) GetValue(ActualUriProperty); }
            set { SetValue(ActualUriProperty, value); }
        }

        public static readonly DependencyProperty PageLoadedCommandProperty = DependencyProperty.Register(
            "PageLoadedCommand", typeof (ICommand), typeof (WebViewBehaviour), new PropertyMetadata(default(ICommand)));

        public ICommand PageLoadedCommand
        {
            get { return (ICommand) GetValue(PageLoadedCommandProperty); }
            set { SetValue(PageLoadedCommandProperty, value); }
        }
        
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.NavigationStarting += WebView_OnNavigationStarting;
            AssociatedObject.NavigationFailed += WebView_OnNavigationFailed;
            AssociatedObject.LoadCompleted += WebView_OnLoadCompleted;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.NavigationStarting -= WebView_OnNavigationStarting;
            AssociatedObject.NavigationFailed -= WebView_OnNavigationFailed;
            AssociatedObject.LoadCompleted -= WebView_OnLoadCompleted;
        }

        private static void OnActualUriChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behaviour = sender as WebViewBehaviour;
            if (behaviour == null) return;

            if (behaviour.AssociatedObject.Source != behaviour.ActualUri)
            {
                behaviour.AssociatedObject.Source = behaviour.ActualUri;
            }
        }

        private void WebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs e)
        {
            ActualUri = e.Uri;
            IsLoading = true;
        }

        private void WebView_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            ActualUri = e.Uri;
            IsLoading = false;
            if (PageLoadedCommand != null)
            {
                PageLoadedCommand.Execute(null);
            }
        }

        private void WebView_OnNavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            IsLoading = false;
        }
    }
}
