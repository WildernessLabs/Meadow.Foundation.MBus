namespace Meadow.Foundation.MBus;

/// <summary>
/// Represents a control telegram used in MBus communication.
/// </summary>
public record ControlTelegram : Telegram
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ControlTelegram"/> record.
    /// The control telegram is always 9 bytes long.
    /// </summary>
    /// <param name="control">The control byte.</param>
    /// <param name="address">The address byte.</param>
    /// <param name="controlInfo">The control info byte.</param>
    public ControlTelegram(Control control, byte address, byte controlInfo)
        : base(9)
    {
        Data[0] = 0x68;     // start1
        Data[1] = 0x03;     // length1
        Data[2] = Data[1];  // length2 length is duplicated, per the spec
        Data[3] = 0x68;     // start2
        Data[4] = (byte)control;
        Data[5] = address;
        Data[6] = controlInfo;
        Data[7] = CalculateChecksum(Data, 4, 3); // checksum is control + address + control info only
        Data[8] = 0x16;     // stop
    }
}
