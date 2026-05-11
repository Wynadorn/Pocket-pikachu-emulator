using Microsoft.AspNetCore.Components.WebView.Maui;
using PocketPikachuMaui.Interfaces;

namespace PocketPikachuMaui.Services;

public class StepCounterManager
{
    private readonly IStepCounterService _stepCounterService;
    private BlazorWebView? _webView;
    private System.Timers.Timer? _updateTimer;
    private int _lastStepCount = 0;

    public StepCounterManager(IStepCounterService stepCounterService)
    {
        _stepCounterService = stepCounterService;
    }

    public async Task InitializeAsync(BlazorWebView webView)
    {
        _webView = webView;

        // Check if step counter is available
        var isAvailable = await _stepCounterService.IsAvailableAsync();
        if (!isAvailable)
        {
            Console.WriteLine("Step counter not available on this device");
            return;
        }

        // Request permission
        var hasPermission = await _stepCounterService.RequestPermissionAsync();
        if (!hasPermission)
        {
            Console.WriteLine("Step counter permission denied");
            return;
        }

        // Subscribe to step count changes
        _stepCounterService.StepCountChanged += OnStepCountChanged;

        // Start tracking
        await _stepCounterService.StartTrackingAsync();

        // Set up timer to periodically log step count
        _updateTimer = new System.Timers.Timer(5000); // Update every 5 seconds
        _updateTimer.Elapsed += async (s, e) => await LogStepCount();
        _updateTimer.Start();
    }

    public void StartTracking()
    {
        _updateTimer?.Start();
    }

    public void StopTracking()
    {
        _updateTimer?.Stop();
    }

    private void OnStepCountChanged(object? sender, StepCountChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await LogStepCount();
        });
    }

    private async Task LogStepCount()
    {
        if (_webView == null) return;

        try
        {
            var stepCount = await _stepCounterService.GetStepCountAsync();
            var todaySteps = await _stepCounterService.GetTodayStepsAsync();

            if (stepCount != _lastStepCount)
            {
                _lastStepCount = stepCount;
                Console.WriteLine($"Native Step Counter: Total={stepCount}, Today={todaySteps}");

                // TODO: Inject steps into JavaScript once JSInterop is properly configured
                // For now, the user can manually use the shake button in the web app
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading step count: {ex.Message}");
        }
    }
}
