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
        try
        {
            // Get service after the handler is initialized
            _stepCounterManager = Handler?.MauiContext?.Services.GetService<StepCounterManager>();

            if (_stepCounterManager != null)
            {
                await _stepCounterManager.InitializeAsync(blazorWebView);
                _stepCounterManager.StartTracking();
            }
        }
        catch (Exception ex)
        {
            // Log the error but don't crash the app
            Console.WriteLine($"Error initializing step counter: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            // Show user-friendly message
            await DisplayAlert("Step Counter Error",
                $"Failed to initialize step counter: {ex.Message}\n\nThe app will continue without step tracking.",
                "OK");
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
