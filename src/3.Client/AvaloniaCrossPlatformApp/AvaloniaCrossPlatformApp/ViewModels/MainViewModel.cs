using System.Reactive.Linq;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using Splat;

namespace AvaloniaCrossPlatformApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        
        public LoginViewModel LoginViewModel { get; set; } = new LoginViewModel();
    }

    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            
        }
        
        public string LoginID { get; set; }
        public string LoginPW { get; set; }

        private string _loginResultMessage;

        public string LoginResultMessage
        {
            get => _loginResultMessage;
            set
            {
                this.RaiseAndSetIfChanged(ref _loginResultMessage, value); 
            }
        }

        private bool _isLogin = false;

        public bool IsLogin
        {
            get => _isLogin;
            set
            {
                this.RaiseAndSetIfChanged(ref _isLogin, value);
            }
        }

        private InfoBarSeverity _infoBarSeverity = InfoBarSeverity.Informational;
        public InfoBarSeverity InfoBarSeverity
        {
            get => _infoBarSeverity;
            set
            {
                this.RaiseAndSetIfChanged(ref _infoBarSeverity, value);
            }
        }
    }
}