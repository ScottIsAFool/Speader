using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using ScottIsAFool.Windows.Core.Extensions;
using ScottIsAFool.WindowsPhone.Logging;
using Speader.Helpers;
using Speader.Messages;
using Speader.Model;
using Speader.ViewModel;
using Speader.Views;
using Speader.Views.Sources;

namespace Speader
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App
    {
        private TransitionCollection _transitions;

        private readonly ILog _logger;

        public ViewModelLocator Locator => (ViewModelLocator)Current.Resources["Locator"];

        private static ResourceLoader _loader;

        public static ResourceLoader Loader => _loader ?? (_loader = new ResourceLoader());

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;

            _logger = new WinLogger(typeof(App));

            WinLogger.LogConfiguration.LogType = LogType.InMemory;
            WinLogger.LogConfiguration.LoggingIsEnabled = true;
            WinLogger.LogConfiguration.NumberOfRecords = 100;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.FatalException("Unhandled Exception", e.Exception);
            WinLogger.DumpLogsToFile().ConfigureAwait(false);
            e.Handled = true;
        }

#if WINDOWS_PHONE_APP
        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            Messenger.Default.Send(new NotificationMessage(Constants.Messages.RefreshPinMsg));

            if (args != null)
            {
                if (args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
                {
                    var brokerArgs = args as IWebAuthenticationBrokerContinuationEventArgs;
                    if (brokerArgs != null)
                    {
                        if (brokerArgs.WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                        {
                            var url = brokerArgs.WebAuthenticationResult.ResponseData;
                            if (url.Contains(Constants.Pocket.CallBackUri))
                            {
                                Messenger.Default.Send(new NotificationMessage(Constants.Messages.PocketAuthMsg));
                                return;
                            }

                            if (url.Contains(Constants.Readability.CallBackUri))
                            {
                                // speader:ReadabilityAuthorise?oauth_verifier=fEprGt8Z4S&oauth_token=wRNdPqEPHAWUXEBgXF&oauth_callback_confirmed=true
                                var verification = ReadabilityHelpers.GetVerificationToken(brokerArgs.WebAuthenticationResult);
                                Messenger.Default.Send(new NotificationMessage(verification, Constants.Messages.ReadabilityAuthMsg));
                            }
                        }
                    }
                }
                else if (args.Kind == ActivationKind.Protocol)
                {
                    StartServices().ConfigureAwait(false);
                    var protocolArgs = args as ProtocolActivatedEventArgs;
                    if (protocolArgs != null)
                    {
                        var frame = new Frame();
                        var url = protocolArgs.Uri.AbsoluteUri;
                        var uriArgs = protocolArgs.Uri.QueryDictionary();
                        if (url.StartsWith(Constants.UriScheme.Read))
                        {
                            if (Locator.Reader != null)
                            {
                                SendUriSchemeMessage(uriArgs, SchemeType.Read);
                            }

                            frame.Navigate(typeof(ReaderView));
                        }
                        else if (url.StartsWith(Constants.UriScheme.Save))
                        {
                            if (Locator.Main != null)
                            {
                                SendUriSchemeMessage(uriArgs, SchemeType.Save);
                            }

                            frame.Navigate(typeof(MainPage));
                        }
                        else if (url.StartsWith(Constants.UriScheme.Edit))
                        {
                            if (Locator.Edit != null)
                            {
                                SendUriSchemeMessage(uriArgs, SchemeType.Edit);
                            }

                            frame.Navigate(typeof(EditView));
                        }
                        else
                        {
                            frame.Navigate(typeof(MainPage));
                        }

                        Window.Current.Content = frame;
                        Window.Current.Activate();
                    }
                }
            }
        }

        private static void SendUriSchemeMessage(IDictionary<string, string> uriArgs, SchemeType schemeType)
        {
            var content = string.Empty;
            var isUri = false;
            if (uriArgs.ContainsKey("text"))
            {
                content = WebUtility.UrlDecode(uriArgs["text"]);
            }

            if (uriArgs.ContainsKey("url"))
            {
                content = WebUtility.UrlDecode(uriArgs["url"]);
                isUri = true;
            }

            Messenger.Default.Send(new UriSchemeMessage(content, isUri, schemeType));
        }
#endif

        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            var shareOperation = args.ShareOperation;
            if (shareOperation.Data.Contains(StandardDataFormats.WebLink))
            {
                await StartServices();
                if (Locator.Reader != null)
                    Messenger.Default.Send(new ShareMessage(shareOperation));

                var frame = new Frame();
                frame.Navigate(typeof(ReaderView));
                Window.Current.Content = frame;
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.RefreshPinMsg));
            var rootFrame = Window.Current.Content as Frame;

            //var themeManager = Current.Resources["ThemeManager"];
            StartServices().ConfigureAwait(false);

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.CacheSize = 3;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    _transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        _transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += RootFrame_FirstNavigated;
#endif

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            if (!string.IsNullOrEmpty(e.Arguments))
            {
                var query = new Uri(e.Arguments).QueryDictionary();
                if (query.ContainsKey("source"))
                {
                    var source = query["source"];
                    var type = Type.GetType($"Speader.Views.Sources.{source}View");
                    rootFrame.Navigate(type, new NavigationParameters { ShowHomeButton = true });
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private async Task StartServices()
        {
            Locator.Settings.LoadSettings();
            Locator.Auth.StartService();
            await Locator.Cache.StartService();
            Locator.Tile.SetTileTransparency().ConfigureAwait(false);
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;

            if (rootFrame != null)
            {
                rootFrame.ContentTransitions = _transitions ?? new TransitionCollection { new NavigationThemeTransition() };
                rootFrame.Navigated -= RootFrame_FirstNavigated;
            }
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}