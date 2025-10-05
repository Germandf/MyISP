using MyISP.Maui.Models;
using MyISP.Maui.Services;

namespace MyISP.Maui.Views;

public partial class InvoicesPage : ContentPage
{
    private readonly InvoicesService _invoicesService;
    private CancellationTokenSource? _cts;

    public InvoicesPage(InvoicesService invoicesService)
    {
        InitializeComponent();
        _invoicesService = invoicesService;
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
            var months = 12;
            if (int.TryParse(MonthsEntry.Text, out var parsed))
                months = parsed;

            var data = await _invoicesService.GetMyInvoicesAsync(months, _cts.Token);
            if (data == null)
            {
                ErrorLabel.Text = "Not authorized or failed to load";
                ErrorLabel.IsVisible = true;
                PendingList.ItemsSource = null;
                PaidList.ItemsSource = null;
            }
            else
            {
                PendingList.ItemsSource = data.Pending;
                PaidList.ItemsSource = data.Paid;
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

    private async void OnLoadClicked(object? sender, EventArgs e)
    {
        await LoadAsync();
    }

    private async void OnRefresh(object? sender, EventArgs e)
    {
        await LoadAsync();
    }
}
