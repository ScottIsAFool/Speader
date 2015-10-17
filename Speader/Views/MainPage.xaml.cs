using GalaSoft.MvvmLight.Messaging;

namespace Speader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void InitialiseOnBack()
        {
            Messenger.Default.Send(new NotificationMessage(Constants.Messages.ReloadInProgressMsg));
            base.InitialiseOnBack();
        }
    }
}
