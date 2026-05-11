using PocketPikachuMaui.Services;
using Microsoft.Extensions.DependencyInjection;

namespace PocketPikachuMaui;

public partial class MainPage : ContentPage
{
    private StepCounterManager? _stepCounterManager;

    public MainPage()
    {
        InitializeComponent();
        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        // Get service after the handler is initialized
        _stepCounterManager = Handler?.MauiContext?.Services.GetService<StepCounterManager>();

        if (_stepCounterManager != null)
        {
            await _stepCounterManager.InitializeAsync(blazorWebView);
            _stepCounterManager.StartTracking();
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _stepCounterManager?.StartTracking();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _stepCounterManager?.StopTracking();
    }
}
