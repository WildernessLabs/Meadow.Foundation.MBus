namespace Meadow.MBus;

/// <summary>
/// Interface representing a generic M-Bus telegram.
/// </summary>
public interface ITelegram
{
    /// <summary>
    /// Serializes the telegram into a byte array for transmission.
    /// </summary>
    /// <returns>A byte array representing the serialized telegram.</returns>
    byte[] Serialize();
}
