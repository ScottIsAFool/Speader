using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Insta.Portable;
using Microsoft.Practices.ServiceLocation;
using PocketSharp;
using Readability;
using ScottIsAFool.WindowsPhone.Logging;
using Speader.Design;
using Speader.Helpers;
using Speader.Interfaces;
using Speader.Services;
using INavigationService = Speader.Interfaces.INavigationService;
using NavigationService = Speader.Services.NavigationService;

namespace Speader.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            {
                if (!SimpleIoc.Default.IsRegistered<IAuthenticationService>())
                    SimpleIoc.Default.Register<IAuthenticationService, AuthenticationServiceDesign>();

                if (!SimpleIoc.Default.IsRegistered<ICacheService>())
                    SimpleIoc.Default.Register<ICacheService, CacheServiceDesign>();

                if(!SimpleIoc.Default.IsRegistered<IPocketClient>())
                    SimpleIoc.Default.Register<IPocketClient, PocketClientDesign>();

                if (!SimpleIoc.Default.IsRegistered<IInstapaperClient>())
                    SimpleIoc.Default.Register<IInstapaperClient, InstapaperClientDesign>();

                if (!SimpleIoc.Default.IsRegistered<IReadabilityClient>())
                    SimpleIoc.Default.Register<IReadabilityClient, ReadabilityClientDesign>();

                if(!SimpleIoc.Default.IsRegistered<INavigationService>())
                    SimpleIoc.Default.Register<INavigationService, NavigationServiceDesign>();

                if(!SimpleIoc.Default.IsRegistered<IStorageService>())
                    SimpleIoc.Default.Register<IStorageService, StorageServiceDesign>();

                if(!SimpleIoc.Default.IsRegistered<ISettingsService>())
                    SimpleIoc.Default.Register<ISettingsService, SettingsServiceDesign>();

                if(!SimpleIoc.Default.IsRegistered<ILog>())
                    SimpleIoc.Default.Register<ILog, NullLogger>();

                if(!SimpleIoc.Default.IsRegistered<IApplicationSettingsServiceHandler>())
                    SimpleIoc.Default.Register<IApplicationSettingsServiceHandler, ApplicationSettingsServiceHandlerDesign>();

                if(!SimpleIoc.Default.IsRegistered<ITileService>())
                    SimpleIoc.Default.Register<ITileService, TileServiceDesign>();

                if(!SimpleIoc.Default.IsRegistered<IMessageBoxService>())
                    SimpleIoc.Default.Register<IMessageBoxService, MessageBoxServiceDesign>();

                if(!SimpleIoc.Default.IsRegistered<ILocalisationLoader>())
                    SimpleIoc.Default.Register<ILocalisationLoader, LocalisationLoaderDesign>();

                if(!SimpleIoc.Default.IsRegistered<IReaderHelper>())
                    SimpleIoc.Default.Register<IReaderHelper, ReaderHelperDesign>();
            }
            else
            {
                if (!SimpleIoc.Default.IsRegistered<IAuthenticationService>())
                    SimpleIoc.Default.Register<IAuthenticationService, AuthenticationService>();

                if (!SimpleIoc.Default.IsRegistered<ICacheService>())
                    SimpleIoc.Default.Register<ICacheService, CacheService>();

                if (!SimpleIoc.Default.IsRegistered<IPocketClient>())
#if WINDOWS_PHONE_APP
                    SimpleIoc.Default.Register<IPocketClient>(() => new PocketClient(Constants.Pocket.PocketConsumerKey, callbackUri: Constants.Pocket.CallBackUri, timeout: 30, parserUri: new Uri("http://text.readitlater.com/v3beta/text?output=json", UriKind.Absolute)));
#else
                    SimpleIoc.Default.Register<IPocketClient>(() => new PocketClient(Constants.Pocket.PocketConsumerKey, callbackUri: Constants.Pocket.CallBackUri, timeout: 30, parserUri: new Uri("http://text.readitlater.com/v3beta/text?output=json", UriKind.Absolute), useInsideWebAuthenticationBroker: true, isMobileClient: false));
#endif

                if (!SimpleIoc.Default.IsRegistered<IReadabilityClient>())
                    SimpleIoc.Default.Register<IReadabilityClient>(() => new ReadabilityClient(Constants.Readability.ConsumerKey, Constants.Readability.ConsumerSecret));

                if (!SimpleIoc.Default.IsRegistered<IInstapaperClient>())
                    SimpleIoc.Default.Register<IInstapaperClient>(() => new InstapaperClient(Constants.Instapaper.ConsumerKey, Constants.Instapaper.ConsumerSecret));

                if (!SimpleIoc.Default.IsRegistered<INavigationService>())
                    SimpleIoc.Default.Register<INavigationService, NavigationService>();

                if (!SimpleIoc.Default.IsRegistered<IStorageService>())
                    SimpleIoc.Default.Register<IStorageService, StorageService>();

                if (!SimpleIoc.Default.IsRegistered<ISettingsService>())
                    SimpleIoc.Default.Register<ISettingsService, SettingsService>();

                if (!SimpleIoc.Default.IsRegistered<ILog>())
                    SimpleIoc.Default.Register<ILog>(() => new WinLogger("ServiceLogger"));

                if (!SimpleIoc.Default.IsRegistered<IApplicationSettingsServiceHandler>())
                    SimpleIoc.Default.Register(() => new ApplicationSettingsService().Roaming);

                if (!SimpleIoc.Default.IsRegistered<ITileService>())
                    SimpleIoc.Default.Register<ITileService, TileService>();

                if (!SimpleIoc.Default.IsRegistered<IMessageBoxService>())
                    SimpleIoc.Default.Register<IMessageBoxService, MessageBoxService>();

                if (!SimpleIoc.Default.IsRegistered<ILocalisationLoader>())
                    SimpleIoc.Default.Register<ILocalisationLoader, LocalisationLoader>();

                if (!SimpleIoc.Default.IsRegistered<IReaderHelper>())
                    SimpleIoc.Default.Register<IReaderHelper, ReaderHelper>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ReaderViewModel>(true);
            SimpleIoc.Default.Register<EditViewModel>();
            SimpleIoc.Default.Register<FullPageViewModel>();
            SimpleIoc.Default.Register<PocketViewModel>(true);
            SimpleIoc.Default.Register<ReadabilityViewModel>(true);
            SimpleIoc.Default.Register<InstapaperViewModel>(true);
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<LocalViewModel>(true);
            SimpleIoc.Default.Register<WebViewModel>();
            SimpleIoc.Default.Register<AboutViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public PocketViewModel Pocket => ServiceLocator.Current.GetInstance<PocketViewModel>();

        public ReadabilityViewModel Readability => ServiceLocator.Current.GetInstance<ReadabilityViewModel>();

        public InstapaperViewModel Instapaper => ServiceLocator.Current.GetInstance<InstapaperViewModel>();

        public ReaderViewModel Reader => ServiceLocator.Current.GetInstance<ReaderViewModel>();

        public FullPageViewModel FullPage => ServiceLocator.Current.GetInstance<FullPageViewModel>();

        public SettingsViewModel SettingsVm => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        public LocalViewModel Local => ServiceLocator.Current.GetInstance<LocalViewModel>();

        public WebViewModel Web => ServiceLocator.Current.GetInstance<WebViewModel>();

        public EditViewModel Edit => ServiceLocator.Current.GetInstance<EditViewModel>();

        public AboutViewModel About => ServiceLocator.Current.GetInstance<AboutViewModel>();

        public IAuthenticationService Auth => ServiceLocator.Current.GetInstance<IAuthenticationService>();

        public ICacheService Cache => ServiceLocator.Current.GetInstance<ICacheService>();

        public ISettingsService Settings => ServiceLocator.Current.GetInstance<ISettingsService>();

        public ITileService Tile => ServiceLocator.Current.GetInstance<ITileService>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}