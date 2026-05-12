using PocketPikachuMaui.Interfaces;

namespace PocketPikachuMaui.Services;

/// <summary>
/// Stub implementation for platforms that don't support step counting
/// </summary>
public class StepCounterServiceStub : IStepCounterService
{
    public event EventHandler<StepCountChangedEventArgs>? StepCountChanged;

    public Task<int> GetStepCountAsync() => Task.FromResult(0);

    public Task<int> GetTodayStepsAsync() => Task.FromResult(0);

    public Task<bool> IsAvailableAsync() => Task.FromResult(false);

    public Task<bool> RequestPermissionAsync() => Task.FromResult(false);

    public Task StartTrackingAsync() => Task.CompletedTask;

    public Task StopTrackingAsync() => Task.CompletedTask;
}
