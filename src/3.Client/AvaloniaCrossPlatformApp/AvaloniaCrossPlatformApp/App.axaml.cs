using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaCrossPlatformApp.ViewModels;
using AvaloniaCrossPlatformApp.Views;
using Splat;

namespace AvaloniaCrossPlatformApp
{
    public class App : Avalonia.Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel()
                    {
                        LoginViewModel = new LoginViewModel()
                    }
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = new MainViewModel()
                    {
                        LoginViewModel = new LoginViewModel()
                    }
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}