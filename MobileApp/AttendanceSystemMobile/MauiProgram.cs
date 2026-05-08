using AttendanceSystemMobile.Services;
using AttendanceSystemMobile.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ZXing.Net.Maui.Controls;
using AttendanceSystemMobile.Pages;


namespace AttendanceSystemMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()

                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            var connectionString = Data.DatabaseConfig.GetConnectionString();
             builder.Services.AddDbContext<Data.AppDbContext>(options =>
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

            builder.Services.AddScoped<DatabaseService>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ScannerPage>();
            builder.Services.AddTransient<InitializePasswordPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
