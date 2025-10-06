using MyISP.Maui.Services;

namespace MyISP.Maui
{
    public partial class AppShell : Shell
    {
        private readonly AuthService _auth;

        public AppShell(AuthService auth)
        {
            InitializeComponent();
            _auth = auth;
            _auth.LoggedOut += async (_, __) =>
            {
                try
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await GoToAsync("//Login");
                    });
                }
                catch { }
            };

            Navigating += OnShellNavigating;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                await GoToAsync("//Login");
            }
            else if (!await _auth.IsTokenValidAsync(token))
            {
                await _auth.LogoutAsync();
                await GoToAsync("//Login");
            }
            else
            {
                await GoToAsync("//MyServices");
            }
        }

        private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
        {
            // Allow navigation to login/register always
            var target = e.Target?.Location.OriginalString ?? string.Empty;
            if (target.Contains("Login", StringComparison.OrdinalIgnoreCase) || target.Contains("Register", StringComparison.OrdinalIgnoreCase))
                return;

            var token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token) || !await _auth.IsTokenValidAsync(token))
            {
                e.Cancel();
                await _auth.LogoutAsync();
                await GoToAsync("//Login");
            }
        }
    }
}
