using System;

namespace Meadow.Foundation.MBus;

/// <summary>
/// Represents a long telegram used in MBus communication.
/// </summary>
public record LongTelegram : Telegram
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LongTelegram"/> record with a variable length payload.
    /// </summary>
    /// <param name="control">The control byte.</param>
    /// <param name="address">The address byte.</param>
    /// <param name="controlInfo">The control info byte.</param>
    /// <param name="payload">The payload data.</param>
    public LongTelegram(Control control, byte address, byte controlInfo, byte[] payload)
        : base(9 + payload.Length)
    {
        Data[0] = 0x68;                         // start1
        Data[1] = (byte)(payload.Length + 3);   // length1
        Data[2] = Data[1];                      // length2 length is duplicated, per the spec
        Data[3] = 0x68;                         // start2
        Data[4] = (byte)control;
        Data[5] = address;
        Data[6] = controlInfo;

        Array.Copy(payload, 0, Data, 7, payload.Length);

        Data[7 + payload.Length] = CalculateChecksum(Data, 4, 3 + payload.Length);
        Data[8 + payload.Length] = 0x16;     // stop
    }
}
