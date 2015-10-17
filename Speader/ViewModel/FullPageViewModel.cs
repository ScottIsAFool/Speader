using GalaSoft.MvvmLight.Command;
using Speader.Interfaces;
using Speader.Model;
using Speader.Views;
using Speader.Views.Sources;

namespace Speader.ViewModel
{
    public class FullPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationgService;
        private readonly ReaderViewModel _readerViewModel;
        private readonly EditViewModel _editViewModel;

        public FullPageViewModel(
            INavigationService navigationgService,
            ReaderViewModel readerViewModel,
            EditViewModel editViewModel)
        {
            _navigationgService = navigationgService;
            _readerViewModel = readerViewModel;
            _editViewModel = editViewModel;
        }

        public bool CanEdit
        {
            get { return ReaderItem != null && ReaderItem.Source == SourceProvider.Local; }
        }

        public ReaderItem ReaderItem { get; set; }

        public RelayCommand EditCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _editViewModel.ReaderItem = ReaderItem;

                    _navigationgService.Navigate<EditView>();
                });
            }
        }

        public RelayCommand ReadCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _readerViewModel.SetReaderItem(ReaderItem);
                    _navigationgService.Navigate<ReaderView>();
                });
            }
        }
    }
}
