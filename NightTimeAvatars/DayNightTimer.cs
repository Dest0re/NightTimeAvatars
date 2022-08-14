namespace NightTimeAvatars;

internal class DayNightTimer
{
    private readonly TimeOnly _dayStartTime;
    private readonly TimeOnly _nightStartTime;
    
    internal delegate void OnDayHandler();
    internal delegate void OnNightHandler();

    internal event OnDayHandler? OnDayNotify;
    internal event OnNightHandler? OnNightNotify;

    internal DayNightTimer(TimeOnly dayStartTime, TimeOnly nightStartTime)
    {
        _dayStartTime = dayStartTime;
        _nightStartTime = nightStartTime;
    }

    internal async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (DateTime.Now.Hour == _dayStartTime.Hour 
                && DateTime.Now.Minute == _dayStartTime.Minute 
                && _dayStartTime.Second == DateTime.Now.Second)
                OnDayNotify?.Invoke();
            if (DateTime.Now.Hour == _nightStartTime.Hour 
                && DateTime.Now.Minute == _nightStartTime.Minute 
                && _dayStartTime.Second == DateTime.Now.Second)
                OnNightNotify?.Invoke();
            
            await Task.Delay(1000, cancellationToken);
        }
    }
}