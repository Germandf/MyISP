using MyISP.Maui.Services;

namespace MyISP.Maui.Views;

public partial class MyServicesPage : ContentPage
{
    private readonly UserServicesService _userServicesService;

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

    private async Task LoadAsync()
    {
        ErrorLabel.IsVisible = false;
        try
        {
            var data = await _userServicesService.GetMyServicesAsync();
            if (data == null)
            {
                ErrorLabel.Text = "Not authorized or failed to load";
                ErrorLabel.IsVisible = true;
            }
            else
            {
                InternetList.ItemsSource = data.Internet;
                MobileList.ItemsSource = data.Mobile;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }

    private async void OnRefresh(object? sender, EventArgs e)
    {
        await LoadAsync();
    }
}
