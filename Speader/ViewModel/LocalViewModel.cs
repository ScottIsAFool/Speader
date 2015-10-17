using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Speader.Extensions;
using Speader.Interfaces;
using Speader.Model;
using Speader.Views.Sources;

namespace Speader.ViewModel
{
    public class LocalViewModel : ProviderViewModelBase
    {
        private readonly EditViewModel _editViewModel;

        public LocalViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            ICacheService cacheService,
            ITileService tileService,
            ReaderViewModel readerViewModel,
            FullPageViewModel fullPageViewModel,
            EditViewModel editViewModel)
            : base(navigationService, authenticationService, cacheService, tileService, readerViewModel, fullPageViewModel)
        {
            _editViewModel = editViewModel;

            IsPinned = TileService.IsLocalPinned;
        }

        public RelayCommand AddNewItemCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _editViewModel.ReaderItem = ReaderItem.NewLocalItem();

                    NavigationService.Navigate<EditView>();
                });
            }
        }

        protected override async Task ViewLoaded()
        {
            await LoadData(false);
        }

        public override bool IsLoggedIn
        {
            get { return true; }
        }

        public override SourceProvider Provider
        {
            get { return SourceProvider.Local; }
        }

        protected override async Task LoadData(bool isRefresh)
        {
            var response = await CacheService.GetLocalItemsFromCache();
            if (response.ReaderItems.IsNullOrEmpty())
            {
                return;
            }

            //if (Items.IsNullOrEmpty())
            //{
                Items = new ObservableCollection<ReaderItem>(response.ReaderItems);
            //}
        }

        protected override async Task Archive(ReaderItem item)
        {
            await CacheService.DeleteArticle(item);
            Items.Remove(item);
            await CacheService.SaveLocalItems(Items);
        }

        protected override async Task<ReaderItem> GetArticle(ReaderItem item, bool isRefresh = false)
        {
            if (item == null)
            {
                return null;
            }

            var article = await CacheService.GetArticle(item);

            return article;
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.RefreshPinMsg))
                {
                    IsPinned = TileService.IsLocalPinned;
                }
            });

            base.WireMessages();
        }
    }
}
