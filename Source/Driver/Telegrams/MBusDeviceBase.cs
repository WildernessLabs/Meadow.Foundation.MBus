using System;
using System.Threading;

namespace Meadow.Foundation.MBus;

public abstract class MBusDeviceBase : IMBusDevice, IDisposable
{
    public static TimeSpan DefaultMonitorPeriod = TimeSpan.FromSeconds(5);

    public bool IsMonitoring => _refreshTimer != null;

    private Timer? _refreshTimer = null;
    private bool _refreshing;

    protected abstract void DoRefresh();

    public void Refresh()
    {
        DoRefresh();
    }

    public void StartMonitoring()
    {
        StartMonitoring(DefaultMonitorPeriod);
    }

    public void StartMonitoring(TimeSpan period)
    {
        if (_refreshTimer != null) return;

        _refreshTimer = new Timer(TimerProc, null, TimeSpan.Zero, period);
    }

    public void StopMonitoring()
    {
        if (_refreshTimer != null)
        {
            _refreshTimer.Change(-1, -1);
            _refreshTimer.Dispose();
            _refreshTimer = null;
        }
    }

    private void TimerProc(object? state)
    {
        if (_refreshing) return;
        _refreshing = true;
        Refresh();
        _refreshing = false;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _refreshTimer?.Dispose();
    }
}
