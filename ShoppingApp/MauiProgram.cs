using Microsoft.Extensions.Logging;
using ShoppingApp.Services;
using ShoppingApp.ViewModels;
using ShoppingApp.Views;

namespace ShoppingApp
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
                    fonts.AddFont("Jaini-Regular.ttf", "JainiRegular");
                });
            builder.Services.AddSingleton<DatabaseService>();

            builder.Services.AddTransient<CustomerViewModel>();
            builder.Services.AddTransient<CustomerView>();

            builder.Services.AddTransient<ProductViewModel>();
            builder.Services.AddTransient<ProductView>();
            
            builder.Services.AddTransient<ProductDetailsViewModel>();
            builder.Services.AddTransient<ProductDetailsView>();

            builder.Services.AddTransient<CartViewModel>();
            builder.Services.AddTransient<CartView>();

            builder.Services.AddTransient<OrderConfirmationViewModel>();
            builder.Services.AddTransient<OrderConfirmationView>();

            builder.Services.AddTransient<CheckoutViewModel>();
            builder.Services.AddTransient<CheckoutView>();


            builder.Services.AddTransient<AppShell>();
#if DEBUG
            builder.Logging.AddDebug();

            
#endif

            return builder.Build();
        }
    }
}
