using Meadow.Hardware;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Meadow.MBus;

/// <summary>
/// Represents an MBus server using serial communication.
/// </summary>
public class MBusSerialServer : IMBusServer
{
    private byte[] _rxBuffer = new byte[1024];

    private ISerialPort Port { get; }

    /// <summary>
    /// The broadcast address for MBus communication.
    /// </summary>
    public const byte BroadcastAddress = 254;

    /// <summary>
    /// Initializes a new instance of the <see cref="MBusSerialServer"/> class using the specified serial port.
    /// </summary>
    /// <param name="serialPort">The serial port for communication.</param>
    public MBusSerialServer(ISerialPort serialPort)
    {
        Port = serialPort;
    }

    /// <inheritdoc/>
    public void SendLongTelegram(byte clientAddress, byte controlInfo, byte[] payload)
    {
        var telegram = new LongTelegram(Control.SND_UD, clientAddress, controlInfo, payload);
        WriteTelegram(telegram);
    }

    /// <inheritdoc/>
    public void SendControl(byte clientAddress, byte controlInfo)
    {
        var telegram = new ControlTelegram(
            Control.SND_UD | Control.FCB,
            clientAddress,
            controlInfo);

        WriteTelegram(telegram);
    }

    /// <inheritdoc/>
    public byte[] RequestUserData1(byte clientAddress)
    {
        var telegram = new ShortTelegram(
            Control.REQ_UD1 | Control.FCB,
            clientAddress);

        return WriteTelegram(telegram);
    }

    /// <inheritdoc/>
    public byte[] RequestUserData2(byte clientAddress)
    {
        var telegram = new ShortTelegram(
            Control.REQ_UD2 | Control.FCB,
            clientAddress);

        return WriteTelegram(telegram);
    }

    public void InitializeClient(byte clientAddress)
    {
        var frame = new ShortTelegram(Control.SND_NKE, clientAddress);

        WriteTelegram(frame);
    }

    /// <summary>
    /// Writes a telegram to the serial port and waits for the response.
    /// </summary>
    /// <param name="frame">The telegram frame to write.</param>
    /// <returns>The response received from the serial port.</returns>
    public byte[] WriteTelegram(ITelegram frame)
    {
        var frameData = frame.Serialize();

        if (!Port.IsOpen)
        {
            Port.Open();
            Port.ClearReceiveBuffer();
            Port.ReadTimeout = TimeSpan.FromSeconds(1);
        }

        Debug.WriteLine($"TX-Buffer:\r\n{BitConverter.ToString(frameData)}");
        Port.Write(frameData);

        // wait for ack
        try
        {
            Thread.Sleep(2000);
            var r = Port.BytesToRead;

            var read = Port.Read(_rxBuffer, 0, _rxBuffer.Length);

            Debug.WriteLine($"Read: {read}");

            if (read == 0)
            {
                return Array.Empty<byte>();
            }

            // what sort of response do we have?
            if (_rxBuffer[0] == 0xe5)
            {
                // just an ack
                return new byte[1] { _rxBuffer[0] };
            }

            var data = new byte[read];
            Array.Copy(_rxBuffer, data, read);
            Debug.WriteLine($"RX-Buffer:\r\n{BitConverter.ToString(data)}");
            return data;

        }
        catch (IOException)
        {
            // no response
            return Array.Empty<byte>();
        }
    }
}
