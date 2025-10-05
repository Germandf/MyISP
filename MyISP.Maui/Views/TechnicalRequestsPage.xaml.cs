using MyISP.Maui.Models;
using MyISP.Maui.Services;

namespace MyISP.Maui.Views;

public partial class TechnicalRequestsPage : ContentPage
{
    private readonly TechnicalRequestsService _service;
    private List<TechnicalRequestDto> _all = new();

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

    private async Task LoadAsync()
    {
        ErrorLabel.IsVisible = false;
        try
        {
            var data = await _service.GetMyTechnicalRequestsAsync();
            if (data == null)
            {
                ErrorLabel.Text = "Not authorized or failed to load";
                ErrorLabel.IsVisible = true;
            }
            else
            {
                _all = data;
                ActiveList.ItemsSource = _all.Where(t => t.Status != TicketStatus.Closed).ToList();
                CompletedList.ItemsSource = _all.Where(t => t.Status == TicketStatus.Closed).ToList();
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }
}
