using MyISP.Maui.Services;

namespace MyISP.Maui.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;

    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        var user = UsernameEntry.Text?.Trim() ?? string.Empty;
        var pass = PasswordEntry.Text ?? string.Empty;
        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            ErrorLabel.Text = "Enter credentials";
            ErrorLabel.IsVisible = true;
            return;
        }

        try
        {
            var token = await _authService.LoginAsync(user, pass);
            if (string.IsNullOrEmpty(token))
            {
                ErrorLabel.Text = "Invalid credentials";
                ErrorLabel.IsVisible = true;
                return;
            }
            await Shell.Current.GoToAsync("//MyServices");
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }
}
