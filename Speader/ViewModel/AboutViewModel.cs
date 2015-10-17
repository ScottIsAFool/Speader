using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Store;
using Windows.System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Speader.Interfaces;
using Speader.Model;

namespace Speader.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        private readonly ILocalisationLoader _loader;
        private readonly PackageVersion _version;

        public AboutViewModel(ILocalisationLoader loader)
        {
            _loader = loader;
            _version = Package.Current.Id.Version;
        }

        public string Version => $"{_version.Major}.{_version.Minor}";

        public string Build => _version.Build.ToString();

        public string Designer => "Rebecca Chui";

        public string Website => "metronuggets.com";

        public string Twitter => "@scottisafool";

        public string Blog => "metronuggets.com";

        public string SupportEmail => "speaderapp@outlook.com";

        public string EmailSubject => $"{_loader.GetString("SpeaderSupport")} {Version} ({Build})";

        public RelayCommand EmailBryarlyCommand
        {
            get
            {
                return new RelayCommand(() => new EmailComposeService().ShowAsync(EmailSubject, "speaderapp@outlook.com", ""));
            }
        }

        public RelayCommand EmailSupport
        {
            get
            {
                return new RelayCommand(() => new EmailComposeService().ShowAsync(EmailSubject, SupportEmail, ""));
            }
        }

        public RelayCommand DesignerCommand
        {
            get
            {
                return new RelayCommand(() => Launcher.LaunchUriAsync(new Uri("http://uk.linkedin.com/pub/rebecca-chui/30/558/143", UriKind.Absolute)));
            }
        }

        public RelayCommand ViewLoadedCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var manager = DataTransferManager.GetForCurrentView();
                    manager.DataRequested += ManagerOnDataRequested;
                });
            }
        }

        public RelayCommand<string> OpenTwitterCommand
        {
            get
            {
                return new RelayCommand<string>(twitter => Launcher.LaunchUriAsync(new Uri(twitter)));
            }
        }

        public List<TeamMember> TheTeam
        {
            get
            {
                return new List<TeamMember>
                {
                    new TeamMember {AvatarUri = "/Assets/FerretLabs/scott.jpg", Name = "Scott Lovegrove", Twitter = "@scottisafool", TwitterUrl = "http://twitter.com/scottisafool"},
                };
            }
        }

        private void ManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.Properties.Title = _loader.GetString("TellAFriendTitle");

            var messageTemplate = _loader.GetString("TellAFriendMessage");
            var message = string.Format(messageTemplate, CurrentApp.LinkUri);

            request.Data.Properties.Description = message;
            request.Data.SetText(message);
        }

        public RelayCommand BlogCommand
        {
            get
            {
                return new RelayCommand(() => Launcher.LaunchUriAsync(new Uri(Blog, UriKind.Absolute)));
            }
        }

        public RelayCommand WebsiteCommand
        {
            get
            {
                return new RelayCommand(() => Launcher.LaunchUriAsync(new Uri(Website, UriKind.Absolute)));
            }
        }

        public RelayCommand TwitterCommand
        {
            get
            {
                return new RelayCommand(() => Launcher.LaunchUriAsync(new Uri(Twitter, UriKind.Absolute)));
            }
        }

        public RelayCommand TellAFriendCommand
        {
            get
            {
                return new RelayCommand(DataTransferManager.ShowShareUI);
            }
        }

        public RelayCommand ReviewCommand
        {
            get
            {
                return new RelayCommand(() => Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId)));
            }
        }
    }
}
