namespace Meadow.MBus;

/// <summary>
/// Represents a short telegram used in MBus communication.
/// </summary>
public record ShortTelegram : Telegram
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShortTelegram"/> record with a fixed length of 5 bytes.
    /// </summary>
    /// <param name="control">The control byte.</param>
    /// <param name="address">The address byte.</param>
    public ShortTelegram(Control control, byte address)
        : base(5)
    {
        Data[0] = 0x10;     // start
        Data[1] = (byte)control;
        Data[2] = address;
        Data[3] = CalculateChecksum(Data, 1, 2); // checksum is control + address only
        Data[4] = 0x16;     // stop
    }
}
