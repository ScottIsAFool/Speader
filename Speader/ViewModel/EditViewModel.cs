using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Speader.Extensions;
using Speader.Interfaces;
using Speader.Messages;
using Speader.Model;
using Speader.Views;

namespace Speader.ViewModel
{
    public class EditViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ILocalisationLoader _loader;
        private readonly IReaderHelper _readerHelper;
        private readonly ReaderViewModel _readerViewModel;

        public EditViewModel(
            INavigationService navigationService,
            ILocalisationLoader loader,
            IReaderHelper readerHelper,
            ReaderViewModel readerViewModel)
        {
            _navigationService = navigationService;
            _loader = loader;
            _readerHelper = readerHelper;
            _readerViewModel = readerViewModel;
        }

        public ReaderItem ReaderItem { get; set; }

        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await Save(() =>
                    {
                        if (_navigationService.CanGoBack)
                        {
                            _navigationService.GoBack();
                        }
                        else
                        {
                            _navigationService.Navigate<MainPage>(new NavigationParameters{ClearBackstack = true});
                        }
                    });
                });
            }
        }

        public RelayCommand ReadCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await Save();
                    _readerViewModel.SetReaderItem(ReaderItem);
                    _navigationService.Navigate<ReaderView>();
                });
            }
        }

        private async Task Save(Action navigationAction = null)
        {
            var text = _loader.GetString("Saving.Text");
            SetProgressBar(text);

            var copyItem = new ReaderItem();
            ReaderItem.CopyItem(copyItem);
            await _readerHelper.SaveEditedArticle(copyItem);

            SetProgressBar();

            navigationAction?.Invoke();
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<UriSchemeMessage>(this, async m =>
            {
                if (m.SchemeType != SchemeType.Edit)
                {
                    return;
                }
                
                var text = _loader.GetString("Loading");
                SetProgressBar(text);
                
                ReaderItem = await _readerHelper.HandleProtocolMessage(m);

                SetProgressBar();
            });
        }
    }
}
