using System;
using Windows.ApplicationModel.DataTransfer;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Speader.Interfaces;

namespace Speader.ViewModel
{
    public class WebViewModel : ViewModelBase
    {
        private readonly IReaderHelper _readerHelper;

        public WebViewModel(IReaderHelper readerHelper)
        {
            _readerHelper = readerHelper;
        }

        public string Address { get; set; }
        public Uri Url { get; set; }
        public bool IsLoading { get; set; }

        public RelayCommand WebViewLoadedCommand
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

        public RelayCommand NavigateCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (string.IsNullOrEmpty(Address))
                    {
                        return;
                    }

                    if (!Address.ToLower().StartsWith("http") &&
                        !Address.ToLower().StartsWith("https") &&
                        !Address.ToLower().StartsWith("ftp"))
                    {
                        Address = "http://" + Address;
                    }

                    Uri uri;
                    if (Uri.TryCreate(Address, UriKind.RelativeOrAbsolute, out uri))
                    {
                        Url = uri;
                    }
                });
            }
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    SetProgressBar("Getting article");
                    var url = Url;
                    await _readerHelper.SaveArticle(url);

                    SetProgressBar();
                });
            }
        }

        public RelayCommand ShareCommand
        {
            get
            {
                return new RelayCommand(DataTransferManager.ShowShareUI);
            }
        }

        private void ManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.Properties.Title = Url.ToString();
            request.Data.SetUri(Url);
        }

        [UsedImplicitly]
        private void OnUrlChanged()
        {
            Address = Url.AbsoluteUri;
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ResetWebMsg))
                {
                    Address = string.Empty;
                    Url = null;
                }
            });
        }
    }
}
