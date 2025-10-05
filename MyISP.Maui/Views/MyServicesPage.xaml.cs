using MyISP.Maui.Services;

namespace MyISP.Maui.Views;

public partial class MyServicesPage : ContentPage
{
    private readonly UserServicesService _userServicesService;
    private CancellationTokenSource? _cts;

    public MyServicesPage(UserServicesService userServicesService)
    {
        InitializeComponent();
        _userServicesService = userServicesService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async Task LoadAsync()
    {
        ErrorLabel.IsVisible = false;
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        try
        {
            IsBusy = true;
            var data = await _userServicesService.GetMyServicesAsync(_cts.Token);
            if (data == null)
            {
                ErrorLabel.Text = "Not authorized or failed to load";
                ErrorLabel.IsVisible = true;
                InternetList.ItemsSource = null;
                MobileList.ItemsSource = null;
            }
            else
            {
                InternetList.ItemsSource = data.Internet;
                MobileList.ItemsSource = data.Mobile;
            }
        }
        catch (OperationCanceledException)
        {
            // ignore cancel
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
        finally
        {
            IsBusy = false;
            PullToRefresh.IsRefreshing = false;
        }
    }

    private async void OnRefresh(object? sender, EventArgs e)
    {
        await LoadAsync();
    }
}
