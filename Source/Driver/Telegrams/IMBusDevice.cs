using System;

namespace Meadow.MBus;

public interface IMBusDevice
{
    bool IsMonitoring { get; }
    void StartMonitoring(TimeSpan period);
    void StopMonitoring();
    void Refresh();
}
