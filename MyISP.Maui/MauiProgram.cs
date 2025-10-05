using Microsoft.Extensions.Logging;
using MyISP.Maui.Services;

namespace MyISP.Maui
{
    public static class ApiEndpoints
    {
        static string DevHost
        {
            get
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    if (DeviceInfo.DeviceType == DeviceType.Virtual)
                        return "http://10.0.2.2";

                    return $"http://192.168.16.219";
                }
                return "http://localhost";
            }
        }
        static string DevUrl(int port) => $"{DevHost}:{port}";
        public static string IdentityBaseUrl => DevUrl(5253);
        public static string BffBaseUrl => DevUrl(5088);
    }

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
                client.BaseAddress = new Uri(ApiEndpoints.IdentityBaseUrl);
            });
            builder.Services.AddHttpClient("Bff", client =>
            {
                client.BaseAddress = new Uri(ApiEndpoints.BffBaseUrl);
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
