using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Speader.Helpers;
using Speader.Interfaces;
using Speader.Model;
using Speader.Views.Sources;

namespace Speader.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ISettingsService _settingsService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ITileService _tileService;
        private readonly ILocalisationLoader _loader;
        private readonly PocketViewModel _pocketViewModel;
        private readonly ReadabilityViewModel _readabilityViewModel;

        public SettingsViewModel(
            INavigationService navigationService,
            ISettingsService settingsService,
            IAuthenticationService authenticationService,
            ITileService tileService,
            ILocalisationLoader loader,
            PocketViewModel pocketViewModel,
            ReadabilityViewModel readabilityViewModel)
        {
            _navigationService = navigationService;
            _settingsService = settingsService;
            _authenticationService = authenticationService;
            _tileService = tileService;
            _loader = loader;
            _pocketViewModel = pocketViewModel;
            _readabilityViewModel = readabilityViewModel;

            WordsAtATime = _settingsService.WordsAtATime;
            WordsPerMin = _settingsService.WordsPerMin;
            UseTransparentTile = _settingsService.UseTransparentTile;
        }

        public bool UseTransparentTile { get; set; }

        public double WordsPerMin { get; set; }
        public int WordsAtATime { get; set; }
        public ReaderFont ReaderFont { get; set; }

        public string WordsPerMinuteText
        {
            get { return string.Format(_loader.GetString("WordsPerMinuteText"), MathsHelpers.RoundedWordsPerMin(WordsPerMin)); } 
        }

        public string WordsAtATimeText
        {
            get { return WordsAtATime > 1 ? string.Format(_loader.GetString("WordsAtATimeText"), WordsAtATime) : _loader.GetString("WordsAtATimeTextSingle"); }
        }

        public int MinRate
        {
            get { return Constants.MinWordsPerMinute; }
        }

        public int MaxRate
        {
            get { return Constants.MaxWordsPerMinute; }
        }

        public int MinNumberWords
        {
            get { return Constants.MinWordsAtATime; }
        }

        public int MaxNumberWords
        {
            get { return Constants.MaxWordsAtATime; }
        }

        [UsedImplicitly]
        private void OnWordsPerMinChanged()
        {
            _settingsService.WordsPerMin = MathsHelpers.RoundedWordsPerMin(WordsPerMin);
        }

        [UsedImplicitly]
        private void OnWordsAtATimeChanged()
        {
            _settingsService.WordsAtATime = MathsHelpers.RoundedWordsPerMin(WordsAtATime);
        }

        [UsedImplicitly]
        private void OnUseTransparentTileChanged()
        {
            _settingsService.UseTransparentTile = UseTransparentTile;
            _tileService.SetTileTransparency();
        }

        [UsedImplicitly]
        private void OnReaderFontChanged()
        {
            _settingsService.ReaderFont = ReaderFont;
        }

        public RelayCommand<string> ChangeThemeCommand
        {
            get
            {
                return new RelayCommand<string>(theme =>
                {
                    var t = (Theme)Enum.Parse(typeof(Theme), theme, true);
                    _settingsService.Theme = t;
                });
            }
        }

        public RelayCommand<ReaderFont> ChangeFontCommand
        {
            get
            {
                return new RelayCommand<ReaderFont>(font =>
                {
                    ReaderFont = font;
                });
            }
        }

        public RelayCommand LoginWithPocketCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!_authenticationService.IsLoggedInToPocket)
                    {
                        _pocketViewModel.ConnectCommand.Execute(null);
                    }
                });
            }
        }

        public RelayCommand LoginWithReadabilityCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!_authenticationService.IsLoggedIntoReadability)
                    {
                        _readabilityViewModel.ConnectCommand.Execute(null);
                    }
                });
            }
        }

        public RelayCommand LoginWithInstapaperCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!_authenticationService.IsLoggedIntoInstapaper)
                    {
                        _navigationService.Navigate<InstapaperView>();
                    }
                });
            }
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.SaveSettingsMsg))
                {
                    _settingsService.Save();
                }
            });
        }
    }
}
