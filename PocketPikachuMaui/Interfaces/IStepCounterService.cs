namespace PocketPikachuMaui.Interfaces;

public class StepCountChangedEventArgs : EventArgs
{
    public int StepCount { get; set; }
    public int TodaySteps { get; set; }
}

public interface IStepCounterService
{
    Task<bool> IsAvailableAsync();
    Task<bool> RequestPermissionAsync();
    Task StartTrackingAsync();
    Task StopTrackingAsync();
    Task<int> GetStepCountAsync();
    Task<int> GetTodayStepsAsync();
    event EventHandler<StepCountChangedEventArgs>? StepCountChanged;
}
