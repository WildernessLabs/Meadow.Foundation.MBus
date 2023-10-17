namespace Meadow.Foundation.MBus;

/// <summary>
/// Represents an MBus server for communication with MBus devices.
/// </summary>
public interface IMBusServer
{
    /// <summary>
    /// Sends an initialization telegram to the client at the specified address.
    /// </summary>
    /// <param name="clientAddress">The address of the client to send control information to.</param>
    void InitializeClient(byte clientAddress);

    /// <summary>
    /// Sends control information to the client at the specified address.
    /// </summary>
    /// <param name="clientAddress">The address of the client to send control information to.</param>
    /// <param name="controlInfo">The control information to send.</param>
    void SendControl(byte clientAddress, byte controlInfo);

    /// <summary>
    /// Sends a long telegram to the specified client address with the given control information and payload.
    /// </summary>
    /// <param name="clientAddress">The address of the MBus client.</param>
    /// <param name="controlInfo">The control information for the telegram.</param>
    /// <param name="payload">The payload of the telegram.</param>
    void SendLongTelegram(byte clientAddress, byte controlInfo, byte[] payload);

    /// <summary>
    /// Requests user data from the specified MBus client address.
    /// </summary>
    /// <param name="clientAddress">The address of the MBus client.</param>
    /// <returns>The user data received from the client.</returns>
    byte[] RequestUserData2(byte clientAddress);

    /// <summary>
    /// Requests user data from the specified MBus client address.
    /// </summary>
    /// <param name="clientAddress">The address of the MBus client.</param>
    /// <returns>The user data received from the client.</returns>
    byte[] RequestUserData1(byte clientAddress);
}
