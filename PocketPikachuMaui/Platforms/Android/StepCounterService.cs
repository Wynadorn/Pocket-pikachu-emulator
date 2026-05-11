using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using PocketPikachuMaui.Interfaces;

namespace PocketPikachuMaui.Platforms.Android;

public class StepCounterService : Java.Lang.Object, IStepCounterService, ISensorEventListener
{
    private SensorManager? _sensorManager;
    private Sensor? _stepCounterSensor;
    private bool _isTracking = false;
    private int _stepOffset = 0;
    private int _currentSteps = 0;
    private int _todaySteps = 0;
    private DateTime _lastResetDate = DateTime.Today;

    public event EventHandler<StepCountChangedEventArgs>? StepCountChanged;

    public Task<bool> IsAvailableAsync()
    {
        try
        {
            var context = Platform.CurrentActivity ?? Application.Context;
            _sensorManager = (SensorManager?)context.GetSystemService(Context.SensorService);
            _stepCounterSensor = _sensorManager?.GetDefaultSensor(SensorType.StepCounter);
            return Task.FromResult(_stepCounterSensor != null);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<bool> RequestPermissionAsync()
    {
        // On Android 10+ (API 29+), we need ACTIVITY_RECOGNITION permission
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Sensors>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Sensors>();
            }
            return status == PermissionStatus.Granted;
        }
        return true;
    }

    public Task StartTrackingAsync()
    {
        if (_isTracking || _sensorManager == null || _stepCounterSensor == null)
            return Task.CompletedTask;

        // Load saved step offset from preferences
        _stepOffset = Preferences.Get("step_offset", 0);
        _lastResetDate = DateTime.Parse(Preferences.Get("last_reset_date", DateTime.Today.ToString()));

        // Reset today's steps if it's a new day
        if (_lastResetDate.Date != DateTime.Today)
        {
            _todaySteps = 0;
            _lastResetDate = DateTime.Today;
            Preferences.Set("last_reset_date", DateTime.Today.ToString());
        }

        _sensorManager.RegisterListener(this, _stepCounterSensor, SensorDelay.Normal);
        _isTracking = true;

        return Task.CompletedTask;
    }

    public Task StopTrackingAsync()
    {
        if (!_isTracking || _sensorManager == null)
            return Task.CompletedTask;

        _sensorManager.UnregisterListener(this);
        _isTracking = false;

        // Save current offset
        Preferences.Set("step_offset", _stepOffset);

        return Task.CompletedTask;
    }

    public Task<int> GetStepCountAsync()
    {
        return Task.FromResult(_currentSteps);
    }

    public Task<int> GetTodayStepsAsync()
    {
        return Task.FromResult(_todaySteps);
    }

    public void OnAccuracyChanged(Sensor? sensor, [GeneratedEnum] SensorStatus accuracy)
    {
        // Not needed for step counter
    }

    public void OnSensorChanged(SensorEvent? e)
    {
        if (e?.Sensor?.Type != SensorType.StepCounter || e.Values == null)
            return;

        var totalSteps = (int)e.Values[0];

        // Initialize offset on first reading
        if (_stepOffset == 0)
        {
            _stepOffset = totalSteps - Preferences.Get("total_steps", 0);
            Preferences.Set("step_offset", _stepOffset);
        }

        // Calculate current steps
        _currentSteps = totalSteps - _stepOffset;

        // Update today's steps
        var previousTodaySteps = _todaySteps;
        _todaySteps = _currentSteps - Preferences.Get("steps_at_day_start", 0);

        // Save total steps
        Preferences.Set("total_steps", _currentSteps);

        // Notify listeners if steps changed
        if (_todaySteps != previousTodaySteps)
        {
            StepCountChanged?.Invoke(this, new StepCountChangedEventArgs
            {
                StepCount = _currentSteps,
                TodaySteps = _todaySteps
            });
        }
    }
}
