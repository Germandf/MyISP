using MyISP.Maui.Services;

namespace MyISP.Maui.Views;

public partial class ContactInfoPage : ContentPage
{
    private readonly ContactInfoService _service;

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

    private async Task LoadAsync()
    {
        ErrorLabel.IsVisible = false;
        try
        {
            var data = await _service.GetMyContactInfoAsync();
            if (data == null)
            {
                ErrorLabel.Text = "Not authorized or failed to load";
                ErrorLabel.IsVisible = true;
            }
            else
            {
                EmailsList.ItemsSource = data.Emails;
                PhonesList.ItemsSource = data.Phones;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }
}
