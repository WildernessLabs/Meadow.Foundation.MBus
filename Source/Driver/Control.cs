using System;

namespace Meadow.Foundation.MBus;

/// <summary>
/// Control flags for MBus communication.
/// </summary>
[Flags]
public enum Control
{
    /// <summary>
    /// Initialize client (0x40)
    /// </summary>
    SND_NKE = 0b0100_0000,
    /// <summary>
    /// Send User Data to client (0x53/0x73)
    /// </summary>
    SND_UD = 0b0101_0011,
    /// <summary>
    /// Request user Data from client (0x5a/0x7a)
    /// </summary>
    REQ_UD1 = 0b0101_1010,
    /// <summary>
    /// Request user Data from client (0x5b/0x7b)
    /// </summary>
    REQ_UD2 = 0b0101_1011,
    /// <summary>
    /// Client response to User Data request (0x08/0x18/0x28/0x38)
    /// </summary>
    RESP_UD = 0b0000_1000,

    /// <summary>
    /// Frame Count Bit
    /// </summary>
    FCB = 0b0010_0000,


}