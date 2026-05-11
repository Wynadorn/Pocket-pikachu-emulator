using PocketPikachuMaui.Services;

namespace PocketPikachuMaui;

public partial class MainPage : ContentPage
{
    private readonly StepCounterManager _stepCounterManager;

    public MainPage()
    {
        InitializeComponent();

        _stepCounterManager = Handler?.MauiContext?.Services.GetService<StepCounterManager>()
            ?? throw new InvalidOperationException("StepCounterManager not found");

        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        await _stepCounterManager.InitializeAsync(blazorWebView);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _stepCounterManager.StartTracking();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _stepCounterManager.StopTracking();
    }
}
