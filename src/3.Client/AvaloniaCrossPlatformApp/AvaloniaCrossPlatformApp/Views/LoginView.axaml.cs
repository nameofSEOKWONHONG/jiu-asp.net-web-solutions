using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaCrossPlatformApp.ViewModels;
using FluentAvalonia.UI.Controls;
using ReactiveUI;

namespace AvaloniaCrossPlatformApp.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoginClick(object? sender, RoutedEventArgs e)
    {
        var viewModel = (DataContext as LoginViewModel);
        viewModel.InfoBarSeverity = InfoBarSeverity.Error;
        viewModel.LoginResultMessage = $"ID:{viewModel.LoginID}/PW:{viewModel.LoginPW}";
        viewModel.IsLogin = true;
        
        Utils.SetTimeout(1000, () =>
        {
            viewModel.InfoBarSeverity = InfoBarSeverity.Informational;
            viewModel.LoginResultMessage = string.Empty;
            viewModel.IsLogin = false;            
        });
        
    }
}