namespace Meadow.MBus;

/// <summary>
/// Represents an acknowledgment telegram (ACK).
/// </summary>
public record AckTelegram : Telegram
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AckTelegram"/> class.
    /// The acknowledgment telegram is always 1 byte, with a value of 0xE5.
    /// </summary>
    public AckTelegram() : base(new byte[] { 0xe5 })
    {
    }
}
