using MyISP.Maui.Models;
using MyISP.Maui.Services;

namespace MyISP.Maui.Views;

public partial class TechnicalRequestsPage : ContentPage
{
    private readonly TechnicalRequestsService _service;
    private List<TechnicalRequestDto> _all = new();
    private CancellationTokenSource? _cts;

    public TechnicalRequestsPage(TechnicalRequestsService service)
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
            var data = await _service.GetMyTechnicalRequestsAsync(_cts.Token);
            if (data == null)
            {
                ErrorLabel.Text = "Not authorized or failed to load";
                ErrorLabel.IsVisible = true;
                _all = new();
                ActiveList.ItemsSource = null;
                CompletedList.ItemsSource = null;
            }
            else
            {
                _all = data;
                ActiveList.ItemsSource = _all.Where(t => t.Status != TicketStatus.Closed).ToList();
                CompletedList.ItemsSource = _all.Where(t => t.Status == TicketStatus.Closed).ToList();
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
