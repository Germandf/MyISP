using MyISP.Maui.Services;

namespace MyISP.Maui.Views;

public partial class MoreOptionsPage : ContentPage
{
    private readonly AuthService _authService;

    public MoreOptionsPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//Login");
    }
}
