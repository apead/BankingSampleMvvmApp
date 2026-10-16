using BankingApp.Services;
using BankingApp.Views;
using BankingApp.ViewModels;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace BankingApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var culture = new CultureInfo("en-ZA");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
         
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            
            builder.Services.AddSingleton<IBankingDatabaseService, BankingDatabaseService>();
            builder.Services.AddSingleton<INavigationService, ShellNavigationService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();

            builder.Services.AddTransient<CustomerListViewModel>();
            builder.Services.AddTransient<CustomerAccountsViewModel>();
            builder.Services.AddTransient<BankAccountTransactionViewModel>();
            builder.Services.AddTransient<TransactionRecordViewModel>();

            builder.Services.AddTransient<CustomerListView>();
            builder.Services.AddTransient<CustomerAccountsView>();
            builder.Services.AddTransient<BankAccountTransactionView>();
            builder.Services.AddTransient<TransactionRecordView>();

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif WINDOWS
                handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                handler.PlatformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                handler.PlatformView.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
                handler.PlatformView.Resources["TextControlBackgroundFocused"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                handler.PlatformView.Resources["TextControlBackgroundPointerOver"] = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
#elif IOS || MACCATALYST
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });

            return builder.Build();
        }
    }
}
