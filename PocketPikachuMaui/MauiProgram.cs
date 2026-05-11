using Microsoft.Extensions.Logging;
using PocketPikachuMaui.Services;
using PocketPikachuMaui.Interfaces;
using Microsoft.AspNetCore.Components.WebView.Maui;

#if ANDROID
using PocketPikachuMaui.Platforms.Android;
#endif

namespace PocketPikachuMaui;

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
            })
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                // Register custom BlazorWebView handler for JavaScript injection
                handlers.AddHandler<BlazorWebView, CustomBlazorWebViewHandler>();
#endif
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Register platform-specific services
#if ANDROID
        builder.Services.AddSingleton<IStepCounterService, StepCounterService>();
#else
        builder.Services.AddSingleton<IStepCounterService, StepCounterServiceStub>();
#endif
        builder.Services.AddSingleton<StepCounterManager>();

        return builder.Build();
    }
}
