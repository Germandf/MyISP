using MyISP.Maui.Models;
using MyISP.Maui.Services;

namespace MyISP.Maui.Views;

public partial class InvoicesPage : ContentPage
{
    private readonly InvoicesService _invoicesService;

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

    private async Task LoadAsync()
    {
        ErrorLabel.IsVisible = false;
        try
        {
            var months = 12;
            if (int.TryParse(MonthsEntry.Text, out var parsed))
                months = parsed;

            var data = await _invoicesService.GetMyInvoicesAsync(months);
            if (data == null)
            {
                ErrorLabel.Text = "Not authorized or failed to load";
                ErrorLabel.IsVisible = true;
            }
            else
            {
                PendingList.ItemsSource = data.Pending;
                PaidList.ItemsSource = data.Paid;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }

    private async void OnLoadClicked(object? sender, EventArgs e)
    {
        await LoadAsync();
    }
}
