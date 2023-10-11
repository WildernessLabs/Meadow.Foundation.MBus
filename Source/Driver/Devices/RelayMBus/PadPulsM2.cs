using System;
using System.Threading;

namespace Meadow.MBus.Devices.RelayMBus;

/// <summary>
/// Represents a Relay M-Bus PadPuls M2 device for MBus communication.
/// </summary>
public partial class PadPulsM2 : IDisposable
{
    private const byte Address = 0xfe;
    private const byte CI_SelectPort = 0x51;

    private readonly IMBusServer _server;
    private Timer _refreshTimer;
    private bool _refreshing;

    internal enum PortIdentifier : byte
    {
        Port1 = 0,
        Port2 = 1,
    }

    /// <summary>
    /// Gets an array of ports associated with the PadPuls M2 device.
    /// </summary>
    public Port[] Ports { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PadPulsM2"/> class with the specified IMBusServer.
    /// </summary>
    /// <param name="server">The IMBusServer for communication.</param>
    public PadPulsM2(IMBusServer server)
    {
        _server = server;

        Ports = new Port[]
            {
                new Port(PortIdentifier.Port1),
                new Port(PortIdentifier.Port2),
            };

        _refreshTimer = new Timer(TimerProc, null, 0, 5000);
    }

    private void TimerProc(object? state)
    {
        if (_refreshing) return;
        _refreshing = true;
        foreach (var p in Ports)
        {
            RefreshPort(p);
        }
        _refreshing = false;
    }

    private void RefreshPort(Port port)
    {
        SelectPort(port.PortIdentifier);
        var info = GetSelectedPortInfo();
        port.Update(info);
    }

    private void SelectPort(PortIdentifier port)
    {
        // TX
        //             C  A  CI DIF VIF Anw  CS  STOP
        // 68 06 06 68 53 FE 51 01  7F  01   23  16
        _server.SendLongTelegram(Address, CI_SelectPort, new byte[] { 0x01, 0x7f, (byte)port });
    }

    private byte[] GetSelectedPortInfo()
    {
        // TX
        // 10 7B FE 79 16
        // RX
        // 68 2F 2F 68 08 00 72 02 34 79 18 AC 48 42 00 02 00 00 00 0C 00 00 00 00 00 04 6D 00 00 E5 27 42 6C 00 00 4C 00 00 00 00 00 42 EC 7E 01 31 0F 41 01 01 00 2C 16 
        var response = _server.RequestUserData2(Address);

        return response;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _refreshTimer.Dispose();
    }
}
