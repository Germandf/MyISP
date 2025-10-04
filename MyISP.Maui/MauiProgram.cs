using Microsoft.Extensions.Logging;
using MyISP.Maui.Services;

namespace MyISP.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // HttpClients
            builder.Services.AddHttpClient("Identity", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7076");
            });
            builder.Services.AddHttpClient("Bff", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7162");
            });

            // Services
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<WeatherService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
