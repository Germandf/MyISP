using MyISP.Maui.Services;

namespace MyISP.Maui.Views;

public partial class ContactInfoPage : ContentPage
{
    private readonly ContactInfoService _service;
    private CancellationTokenSource? _cts;

    public ContactInfoPage(ContactInfoService service)
    {
        InitializeComponent();
        _service = service;
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
            var data = await _service.GetMyContactInfoAsync(_cts.Token);
            if (data == null)
            {
                ErrorLabel.Text = "Not authorized or failed to load";
                ErrorLabel.IsVisible = true;
                EmailsList.ItemsSource = null;
                PhonesList.ItemsSource = null;
            }
            else
            {
                EmailsList.ItemsSource = data.Emails;
                PhonesList.ItemsSource = data.Phones;
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
