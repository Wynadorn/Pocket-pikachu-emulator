using Microsoft.AspNetCore.Components.WebView.Maui;
using PocketPikachuMaui.Interfaces;
using System.Text.Json;

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

        // Set up timer to periodically update the WebView
        _updateTimer = new System.Timers.Timer(2000); // Update every 2 seconds
        _updateTimer.Elapsed += async (s, e) => await UpdateWebView();
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
            await UpdateWebView();
        });
    }

    private async Task UpdateWebView()
    {
        if (_webView == null) return;

        try
        {
            var stepCount = await _stepCounterService.GetStepCountAsync();

            if (stepCount != _lastStepCount)
            {
                _lastStepCount = stepCount;

                // Inject JavaScript to update step count
                var script = $@"
                    if (typeof pokeStatus !== 'undefined') {{
                        var newSteps = {stepCount};
                        var stepDiff = newSteps - (pokeStatus.steps || 0);
                        if (stepDiff > 0) {{
                            for (var i = 0; i < stepDiff && i < 10; i++) {{
                                if (typeof shakeButtonAction === 'function') {{
                                    shakeButtonAction();
                                }}
                            }}
                        }}
                        console.log('Updated steps from native: ' + newSteps);
                    }}
                ";

                await _webView.EvaluateJavaScriptAsync(script);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating WebView: {ex.Message}");
        }
    }
}
