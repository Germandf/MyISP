using MyISP.Maui.Services;

namespace MyISP.Maui.Views;

public partial class ForecastPage : ContentPage
{
    private readonly WeatherService _weatherService;

    public ForecastPage(WeatherService weatherService)
    {
        InitializeComponent();
        _weatherService = weatherService;
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
            var data = await _weatherService.GetForecastAsync();
            if (data == null)
            {
                ErrorLabel.Text = "Not authorized or failed to load";
                ErrorLabel.IsVisible = true;
            }
            else
            {
                ForecastList.ItemsSource = data;
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
