using MyISP.Maui.Services;

namespace MyISP.Maui.Views;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService;

    public RegisterPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        var email = EmailEntry.Text?.Trim();
        var pass = PasswordEntry.Text;
        var confirm = ConfirmEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass) || pass != confirm)
        {
            ErrorLabel.Text = "Fill all fields and confirm the password";
            ErrorLabel.IsVisible = true;
            return;
        }

        try
        {
            var ok = await _authService.RegisterAsync(email!, pass!);
            if (!ok)
            {
                ErrorLabel.Text = "Registration failed";
                ErrorLabel.IsVisible = true;
                return;
            }
            await Shell.Current.GoToAsync("///Login");
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }

    private async void OnGoToLoginClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///Login");
    }
}
