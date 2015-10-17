using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Speader.Controls
{
    public class IsLoggedInControl : ContentControl
    {
        private Button _button;
        private MenuFlyoutItem _menu;
        
        public static readonly DependencyProperty IsLoggedInProperty = DependencyProperty.Register(
            "IsLoggedIn", typeof(bool), typeof(IsLoggedInControl), new PropertyMetadata(default(bool), OnStateChanged));

        public bool IsLoggedIn
        {
            get { return (bool) GetValue(IsLoggedInProperty); }
            set { SetValue(IsLoggedInProperty, value); }
        }

        public static readonly DependencyProperty LoggedInNameProperty = DependencyProperty.Register(
            "LoggedInName", typeof(string), typeof(IsLoggedInControl), new PropertyMetadata(default(string), OnStateChanged));

        public string LoggedInName
        {
            get { return (string) GetValue(LoggedInNameProperty); }
            set { SetValue(LoggedInNameProperty, value); }
        }

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            "DisplayText", typeof (string), typeof (IsLoggedInControl), new PropertyMetadata(default(string)));

        public string DisplayText
        {
            get { return (string) GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        public static readonly DependencyProperty LogoutCommandProperty = DependencyProperty.Register(
            "LogoutCommand", typeof (ICommand), typeof (IsLoggedInControl), new PropertyMetadata(default(ICommand)));

        public ICommand LogoutCommand
        {
            get { return (ICommand) GetValue(LogoutCommandProperty); }
            set { SetValue(LogoutCommandProperty, value); }
        }

        public static readonly DependencyProperty LoginCommandProperty = DependencyProperty.Register(
            "LoginCommand", typeof (ICommand), typeof (IsLoggedInControl), new PropertyMetadata(default(ICommand)));

        public ICommand LoginCommand
        {
            get { return (ICommand) GetValue(LoginCommandProperty); }
            set { SetValue(LoginCommandProperty, value); }
        }

        private static void OnStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var loggedIn = sender as IsLoggedInControl;

            loggedIn?.SetText();
        }

        public IsLoggedInControl()
        {
            DefaultStyleKey = typeof (IsLoggedInControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetText();

            _button = GetTemplateChild("Button") as Button;

            if (_button != null)
            {
                _button.Tapped -= ButtonOnTapped;
                _button.Tapped += ButtonOnTapped;
            }
        }

        private void ButtonOnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            if (IsLoggedIn)
            {
                FlyoutBase.ShowAttachedFlyout(_button);
            }
            else
            {
                if (LoginCommand != null)
                {
                    LoginCommand.Execute(null);
                }
            }
        }

        private void SetText()
        {
            var localisedText = App.Loader.GetString("LoginText");
            if (!IsLoggedIn)
            {
                DisplayText = localisedText;
                return;
            }

            DisplayText = LoggedInName;
        }
    }
}
