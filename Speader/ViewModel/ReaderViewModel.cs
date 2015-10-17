using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Speader.Extensions;
using Speader.Helpers;
using Speader.Interfaces;
using Speader.Messages;
using Speader.Model;
using Speader.Views;
using INavigationService = Speader.Interfaces.INavigationService;

namespace Speader.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ReaderViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ISettingsService _settingsService;
        private readonly IApplicationSettingsServiceHandler _roamingSettings;
        private readonly ICacheService _cacheService;
        private readonly IMessageBoxService _messageBox;
        private readonly ILocalisationLoader _loader;
        private readonly IReaderHelper _readerHelper;

        private int _wordsRemaining;
        private double _milliseconds;
        private bool _canStart;

        private DispatcherTimer _readerTimer;

        private IList<ReaderItem> _inProgressItems;

        private ShareOperation _shareOperation;

        /// <summary>
        /// Initializes a new instance of the ReaderViewModel class.
        /// </summary>
        public ReaderViewModel(
            INavigationService navigationService,
            ISettingsService settingsService,
            IApplicationSettingsServiceHandler roamingSettings,
            ICacheService cacheService,
            IMessageBoxService messageBox,
            ILocalisationLoader loader,
            IReaderHelper readerHelper)
        {
            _navigationService = navigationService;
            _settingsService = settingsService;
            _roamingSettings = roamingSettings;
            _cacheService = cacheService;
            _messageBox = messageBox;
            _loader = loader;
            _readerHelper = readerHelper;
        }

        public bool IsSavingVisible { get; set; }
        public bool HomeButtonIsVisible { get; set; }

        public ReaderFont ReaderFont { get; set; }
        public ReaderItem SelectedItem { get; set; }
        public List<string> WordsList { get; set; }
        public bool IsPaused { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
        public double ReadPercentage { get; set; }

        public bool ShowFinishedScreen { get; set; }
        public string FinishedText { get; set; }

        public string TimeRemainingText => TimeRemaining.HasValue ? $"{TimeRemaining.Value.ToFriendlyText()} left" : string.Empty;

        public int SelectedIndex { get; set; }
        public string DisplayWords { get; set; }
        public int WordsPerMinute { get; set; }

        public string WordsPerMinuteText => string.Format(_loader.GetString("WordsPerMinuteText"), MathsHelpers.RoundedWordsPerMin(WordsPerMinute));

        public bool CanSkip => WordsList != null && SelectedIndex < WordsList.Count - 1;

        public bool CanSkipBack => SelectedIndex > 0;

        public bool ShowWordsAMinute => IsPaused && !ShowFinishedScreen;

        private async Task GetInProgressItems()
        {
            var response = await _cacheService.GetInProgressItemsFromCache();

            _inProgressItems = response.ReaderItems;
        }

        public async Task SetReaderItem(ReaderItem item, bool isRestarting = false, bool autostart = true)
        {
            ReaderFont = _settingsService.ReaderFont;
            SelectedItem = item;
            GetInProgressItems().ConfigureAwait(false);
            AddToRecent().ConfigureAwait(false);
            IsPaused = false;
            if (!isRestarting)
            {
                _canStart = false;
            }

            WordsList = null;
            TimeRemaining = null;
            _readerTimer = null;
            WordsPerMinute = _settingsService.WordsPerMin;
            SelectedIndex = -1;
            SetProgressBar("Preparing article...");

            SelectedItem.WordsRead = GetWordsRead();

            var words = item.Text.Clean().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (_settingsService.WordsAtATime == 1)
            {
                WordsList = words.ToList();
                SetTimings();
                SetProgressBar();
            }
            else
            {
                var list = words.ToWordList(_settingsService.WordsAtATime);
                WordsList = list;
                SetTimings();
                SetProgressBar();
            }

            if (_canStart && autostart)
            {
                StartStopTimer();
                return;
            }
            _canStart = true;
        }

        public RelayCommand PreviousCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedIndex > 0)
                    {
                        SelectedIndex--;
                    }
                });
            }
        }

        public RelayCommand PlayCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (ShowFinishedScreen)
                    {
                        return;
                    }

                    PlayPause();
                });
            }
        }

        public RelayCommand PlayRestartCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (ShowFinishedScreen)
                    {
                        SelectedItem.WordsRead = 0;
                        ReadPercentage = 0;
                        SetReaderItem(SelectedItem, true).ConfigureAwait(false);
                    }
                    else
                    {
                        PlayPause();
                    }
                });
            }
        }

        private void PlayPause()
        {
            IsPaused = !IsPaused;

            if (_readerTimer == null)
            {
                CreateTimer();
            }

            StartStopTimer();
        }

        private void StartStopTimer()
        {
            if (IsPaused)
            {
                if (_readerTimer.IsEnabled)
                {
                    _readerTimer.Stop();
                }

                SetTimeRemaining();
            }
            else
            {
                if (!_readerTimer.IsEnabled)
                {
                    _readerTimer.Start();
                }
            }
        }

        public RelayCommand NextCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedIndex > 0)
                    {
                        SelectedIndex++;
                    }
                });
            }
        }

        public RelayCommand DoneCommand => new RelayCommand(Done);

        public RelayCommand HomeCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.Navigate<MainPage>());
            }
        }

        public RelayCommand ReaderViewLoaded
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var manager = DataTransferManager.GetForCurrentView();
                    manager.DataRequested += ManagerOnDataRequested;

                    HomeButtonIsVisible = false;
                    if (_canStart)
                    {
                        StartStopTimer();
                        return;
                    }

                    _canStart = true;
                });
            }
        }

        private void ManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.Properties.Title = SelectedItem.Title;
            var milliseconds = MathsHelpers.DisplayTimePerWordGroup(SelectedItem.WordCount, _settingsService.WordsPerMin, _settingsService.WordsAtATime);
            var time = MathsHelpers.TimeToReadArticle(milliseconds, SelectedItem.WordCount, _settingsService.WordsAtATime);
            var message = string.Format(_loader.GetString("ShareText"), SelectedItem.Title, SelectedItem.Url, time.ToFriendlyText());
            request.Data.Properties.Description = message;
            request.Data.SetText(message);
        }

        public RelayCommand<string> ChangeThemeCommand
        {
            get
            {
                return new RelayCommand<string>(theme =>
                {
                    var t = (Theme)Enum.Parse(typeof(Theme), theme, true);
                    _settingsService.Theme = t;
                    _settingsService.Save();
                });
            }
        }

        public RelayCommand ShareCommand => new RelayCommand(DataTransferManager.ShowShareUI);

        private void SetTimings()
        {
            if (SelectedItem == null)
            {
                return;
            }

            SetTimeRemaining();
            SelectedIndex = (SelectedItem.WordCount - _wordsRemaining) / _settingsService.WordsAtATime;

            CreateTimer();
        }

        private void SetTimeRemaining()
        {
            _wordsRemaining = SelectedItem.WordCount - SelectedItem.WordsRead;
            _milliseconds = MathsHelpers.DisplayTimePerWordGroup(SelectedItem.WordCount, _settingsService.WordsPerMin, _settingsService.WordsAtATime);
            TimeRemaining = MathsHelpers.TimeToReadArticle(_milliseconds, _wordsRemaining, _settingsService.WordsAtATime);
            ReadPercentage = SelectedItem.PercentageRead;
        }

        private void CreateTimer()
        {
            _readerTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(_milliseconds)
            };

            _readerTimer.Tick += ReaderTimerOnTick;
        }

        private void ReaderTimerOnTick(object sender, object o)
        {
            SelectedIndex++;
        }

        [UsedImplicitly]
        private void OnSelectedIndexChanged()
        {
            if (SelectedItem == null)
            {
                return;
            }

            var wordsRead = (SelectedIndex * _settingsService.WordsAtATime);
            SelectedItem.WordsRead = wordsRead + _settingsService.WordsAtATime;
            if (!WordsList.IsNullOrEmpty())
            {
                DisplayWords = WordsList[SelectedIndex];

                if (SelectedIndex == WordsList.Count - 1)
                {
                    IsPaused = true;
                    StartStopTimer();
                    Done();
                    return;
                }

                ShowFinishedScreen = false;
            }
        }

        [UsedImplicitly]
        private void OnWordsPerMinuteChanged()
        {
            _settingsService.WordsPerMin = WordsPerMinute;
            SetTimings();
        }

        private void Done()
        {
            SelectedItem.WordsRead = 0;
            _roamingSettings.Remove(SelectedItem.InternalId);
            ShowFinishedScreen = true;

            var milliseconds = MathsHelpers.DisplayTimePerWordGroup(SelectedItem.WordCount, _settingsService.WordsPerMin, _settingsService.WordsAtATime);
            var time = MathsHelpers.TimeToReadArticle(milliseconds, SelectedItem.WordCount, _settingsService.WordsAtATime);

            var format = SelectedItem.WordCount > 1 ? _loader.GetString("FinishedReadingWordsText") : _loader.GetString("FinishedReadingWordText");
            FinishedText = SelectedItem.WordCount > 1 ? string.Format(format, SelectedItem.WordCount, time.ToFriendlyText()) : string.Format(format, time.ToFriendlyText());
        }

        private int GetWordsRead()
        {
            return _roamingSettings.Get(SelectedItem.InternalId, 0);
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ReaderViewLeftMsg))
                {
                    ShowFinishedScreen = false;
                    if (_readerTimer != null && _readerTimer.IsEnabled)
                    {
                        _readerTimer.Stop();
                    }

                    var wordsRead = SelectedIndex * _settingsService.WordsAtATime;
                    var articleNotFinished = wordsRead < SelectedItem.WordCount;
                    if (articleNotFinished)
                    {
                        _roamingSettings.Set(SelectedItem.InternalId, wordsRead);
                    }
                    else
                    {
                        _roamingSettings.Remove(SelectedItem.InternalId);
                    }

                    SaveInProgressItems(wordsRead, !articleNotFinished);
                }
            });

            Messenger.Default.Register<ShareMessage>(this, async m =>
            {
                _shareOperation = (ShareOperation)m.Sender;
                if (_shareOperation.Data.Contains(StandardDataFormats.WebLink))
                {
                    var url = await _shareOperation.Data.GetWebLinkAsync();
                    var message = new UriSchemeMessage(url.ToString(), true, SchemeType.Read);

                    await SaveItemFromExternal(message, async () =>
                    {
                        var title = _loader.GetString("ShareErrorTitle");
                        await _messageBox.ShowAsync(_loader.GetString("ShareErrorText"), title, new List<string> { _loader.GetString("MessageBoxOk") });
                        _shareOperation.ReportError(title);
                    });
                }
                else
                {
                    _shareOperation.ReportCompleted();
                }
            });

            Messenger.Default.Register<UriSchemeMessage>(this, async m =>
            {
                if (m.SchemeType != SchemeType.Read)
                {
                    return;
                }

                await SaveItemFromExternal(m);
            });
        }

        private async Task SaveItemFromExternal(UriSchemeMessage message, Action errorAction = null)
        {
            HomeButtonIsVisible = true;
            var allOk = true;
            ReaderItem item = null;
            try
            {
                IsSavingVisible = true;

                item = await _readerHelper.HandleProtocolMessage(message);
            }
            catch (Exception ex)
            {
                Log.ErrorException("ShareMessage", ex);
                allOk = false;
            }

            if (item != null)
            {
                await SetReaderItem(item, autostart: false);
            }

            IsSavingVisible = false;


            if (allOk && _canStart)
            {
                StartStopTimer();
            }
            else
            {
                errorAction?.Invoke();
            }
        }

        private void SaveInProgressItems(int wordsRead, bool removeItem = false)
        {
            var readerItem = CopyReaderItem(wordsRead);

            var item = _inProgressItems.FirstOrDefault(x => x.Id == readerItem.Id);
            if (item != null)
            {
                if (removeItem)
                {
                    _inProgressItems.Remove(item);
                }
                else
                {
                    SelectedItem.CopyItem(item);
                }
            }
            else
            {
                _inProgressItems.Insert(0, readerItem);
            }

            _cacheService.SaveInProgressItems(_inProgressItems).ConfigureAwait(false);
        }

        private async Task AddToRecent()
        {
            var readerItem = CopyReaderItem();
            var response = await _cacheService.GetRecentItemsFromCache();

            var itemExists = response.ReaderItems.FirstOrDefault(x => x.Id == readerItem.Id);
            if (itemExists != null)
            {
                response.ReaderItems.Remove(itemExists);
            }

            response.ReaderItems.Insert(0, readerItem);
            if (response.ReaderItems.Count > 20)
            {
                await _cacheService.SaveRecentItems(response.ReaderItems.Take(20).ToList());
            }
            else
            {
                await _cacheService.SaveRecentItems(response.ReaderItems);
            }
        }

        private ReaderItem CopyReaderItem(int? wordsRead = null)
        {
            var readerItem = new ReaderItem();
            SelectedItem.CopyItem(readerItem);
            if (wordsRead.HasValue)
            {
                readerItem.WordsRead = wordsRead.Value;
            }

            if (string.IsNullOrEmpty(readerItem.Excerpt))
            {
                readerItem.Excerpt = readerItem.Text.ToExcerpt();
            }

            readerItem.Text = string.Empty;
            return readerItem;
        }
    }
}