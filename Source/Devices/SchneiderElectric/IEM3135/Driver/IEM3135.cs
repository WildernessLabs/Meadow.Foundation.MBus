using Meadow.Units;
using System;

namespace Meadow.Foundation.MBus.SchneiderElectric;

/// <summary>
/// Represents an iEM3135 MBus device.
/// </summary>
public class IEM3135 : MBusDeviceBase
{
    private IMBusServer _server;

    private byte[]? _telegram1;
    private byte[]? _telegram2;
    private byte[]? _telegram3;

    /// <summary>
    /// Gets the M-Bus address for the device
    /// </summary>
    public byte Address { get; }

    /// <summary>
    /// Gets the phase L1 voltage.
    /// </summary>
    public Voltage? L1Voltage => _telegram1 == null ? null : new Voltage(Convert.ToSingle(Telegram.Decode(_telegram1[142..])));

    /// <summary>
    /// Gets the phase L2 voltage.
    /// </summary>
    public Voltage? L2Voltage => _telegram1 == null ? null : new Voltage(Convert.ToSingle(Telegram.Decode(_telegram1[151..])));

    /// <summary>
    /// Gets the phase L3 voltage.
    /// </summary>
    public Voltage? L3Voltage => _telegram1 == null ? null : new Voltage(Convert.ToSingle(Telegram.Decode(_telegram1[160..])));

    /// <summary>
    /// Gets the mean voltage.
    /// </summary>
    public Voltage? MeanVoltage => _telegram1 == null ? null : new Voltage(Convert.ToSingle(Telegram.Decode(_telegram1[169..])));

    /// <summary>
    /// Gets the phase L1 power.
    /// </summary>
    public Power? L1Power => _telegram1 == null ? null : new Power(Convert.ToSingle(Telegram.Decode(_telegram1[178..])));

    /// <summary>
    /// Gets the phase L2 power.
    /// </summary>
    public Power? L2Power => _telegram1 == null ? null : new Power(Convert.ToSingle(Telegram.Decode(_telegram1[186..])));

    /// <summary>
    /// Gets the phase L3 power.
    /// </summary>
    public Power? L3Power => _telegram1 == null ? null : new Power(Convert.ToSingle(Telegram.Decode(_telegram1[194..])));

    /// <summary>
    /// Gets the total power.
    /// </summary>
    public Power? TotalPower => _telegram1 == null ? null : new Power(Convert.ToSingle(Telegram.Decode(_telegram1[202..])));

    /// <summary>
    /// Gets the reactive power.
    /// </summary>
    public float? ReactivePower => _telegram1 == null ? null : Convert.ToSingle(Telegram.Decode(_telegram1[208..]));

    /// <summary>
    /// Gets the apparent power.
    /// </summary>
    public float? ApparentPower => _telegram1 == null ? null : Convert.ToSingle(Telegram.Decode(_telegram1[215..]));

    /// <summary>
    /// Gets the frequency.
    /// </summary>
    public Frequency? Frequency => _telegram1 == null ? null : new Frequency(Convert.ToSingle(Telegram.Decode(_telegram1[230..])));

    /// <summary>
    /// Gets the total active energy export.
    /// </summary>
    public ulong? TotalActiveEnergyExport => _telegram2 == null ? null : Convert.ToUInt64(Telegram.Decode(_telegram2[19..]));

    /// <summary>
    /// Gets the total reactive energy import.
    /// </summary>
    public ulong? TotalReactiveEnergyImport => _telegram2 == null ? null : Convert.ToUInt64(Telegram.Decode(_telegram2[31..]));

    /// <summary>
    /// Gets the system date and time.
    /// </summary>
    public DateTime? SystemDateTime => _telegram2 == null ? null : Convert.ToDateTime(Telegram.Decode(_telegram2[194..]));

    //var activatedStatus = Convert.ToUInt16(Telegram.Decode(_telegram3[36..]));
    //var unacknowlegedStatus = Convert.ToUInt16(Telegram.Decode(_telegram3[41..]));

    /// <summary>
    /// Gets the meter operation time.
    /// </summary>
    public ulong? MeterOperationTime => _telegram3 == null ? null : Convert.ToUInt64(Telegram.Decode(_telegram3[61..]));

    /// <summary>
    /// Initializes a new instance of the <see cref="IEM3135"/> class with the specified IMBusServer.
    /// </summary>
    /// <param name="server">The IMBusServer for communication.</param>
    /// <param name="deviceAddress">The address of the MBus device.</param>
    public IEM3135(IMBusServer server, byte deviceAddress)
    {
        _server = server;
        Address = deviceAddress;

        _server.InitializeClient(Address);
    }

    /// <inheritdoc/>
    protected override void DoRefresh()
    {
        // device has 3 long telegrams of data
        while (true)
        {
            var telegram = _server.RequestUserData2(Address);

            switch (telegram.Length)
            {
                case 3:
                    break;
                case 250:
                    _telegram1 = telegram;
                    break;
                case 252:
                    _telegram2 = telegram;
                    break;
                case 204:
                    _telegram3 = telegram;
                    return;
            }
        }
    }

    /*
    TELEGRAM 1
    Typical meter query:
    TxD:10 7B 02 7D 16 

         0  1  2  3  4  5  6  7  8  9 10  1  2  3  4  5  6  7  8  9 20  1  2  3  4  5  6  7  8  9 30  1  2  3  4  5  6  7  8  9 40  1  2  3  4  5  6  7  8  9
    RxD:68 F4 F4 68 08 02 72 21 20 12 08 A3 4C 18 02 10 00 00 00 0D FD 0A 12 63 69 72 74 63 65 6C 45 20 72 65 64 69 65 6E 68 63 53 0D FD 0C 08 20 35 33 31 33

        50  1  2  3  4  5  6  7  8  9 60  1  2  3  4  5  6  7  8  9 70  1  2  3  4  5  6  7  8  9 80  1  2  3  4  5  6  7  8  9 90  1  2  3  4  5  6  7  8  9
        4D 45 69 0D FD 0E 07 32 30 30 2E 34 2E 31 03 FD 17 58 00 00 05 FD DC FF 01 00 00 00 00 05 FD DC FF 02 00 00 00 00 05 FD DC FF 03 00 00 00 00 05 FD DC  

       100  1  2  3  4  5  6  7  8  9 110  1  2  3  4  5  6  7  8  9 120  1  2  3  4  5  6  7  8  9 130  1  2  3  4  5  6  7  8  9 140  1  2  3  4  5  6  7  8  9 
        FF 00 00 00 00 00 05 FD C9 FF  05 1F 78 76 42 05 FD C9 FF 06  00 00 00 00 05 FD C9 FF 07 1D  87 76 42 05 FD C9 FF 08 14 55  24 42 05 FD C9 FF 01 2C 47 F6  

       150  1  2  3  4  5  6  7  8  9 160  1  2  3  4  5  6  7  8  9 170  1  2  3  4  5  6  7  8  9 180  1  2  3  4  5  6  7  8  9 190  1  2  3  4  5  6  7  8  9 
        42 05 FD C9 FF 02 6F 2C 76 42  05 FD C9 FF 03 FC 13 76 42 05  FD C9 FF 04 76 22 A4 42 05 AE  FF 01 00 00 00 00 05 AE FF 02  00 00 00 00 05 AE FF 03 00 00  

       200  1  2  3  4  5  6  7  8  9 210  1  2  3  4  5  6  7  8  9 220  1  2  3  4  5  6  7  8  9 230  1  2  3  4  5  6  7  8  9 240  1  2  3  4  5  6  7  8  9        
        00 00 05 2E 00 00 00 00 85 40  2E 00 00 00 00 85 80 40 2E 00  00 00 00 05 FF 0A 00 00 C0 FF  05 FF 0B 51 00 70 42 07 03 00 00  00 00 00 00 00 00 1F 44 16 
    */

    /*
    TELEGRAM 2
    Typical meter query:
    TxD:10 5B 02 5D 16 
    RxD:68 F6 F6 68 08 02 72 21 20 12 08 A3 4C 18 02 11 00 00 00 07 83 FF 09 00 00 00 00 00 00 00 00 87 40 03 00 00 00 00 00 00 00 00 87 40 83 FF 09 00 00 00 00 00 00 00 00 04 ED FF 0C 80 20 01 01 07 83 FF 0D 00 00 00 00 00 00 00 00 87 40 83 FF 0D 00 00 00 00 00 00 00 00 07 83 FF 01 00 00 00 00 00 00 00 00 07 83 FF 02 00 00 00 00 00 00 00 00 07 83 FF 03 00 00 00 00 00 00 00 00 04 ED FF 0E 80 20 01 01 07 FD 61 00 00 00 00 00 00 00 00 03 FF 10 00 00 00 87 10 03 00 00 00 00 00 00 00 00 87 20 03 00 00 00 00 00 00 00 00 87 30 03 00 00 00 00 00 00 00 00 87 80 10 03 00 00 00 00 00 00 00 00 04 6D B2 20 01 01 03 FF 2C 64 00 00 03 FF 2D 00 00 00 05 FF 2E 00 00 C8 42 05 FF 2F 00 00 FA 43 03 FF 30 00 00 00 03 FD 1B 00 00 00 02 FF 32 00 00 03 FD 1A FF FF 00 1F 62 16 
    */


    /*
    TELEGRAM 3
    Typical meter query:
    TxD:10 7B 02 7D 16 
    RxD:68 C6 C6 68 08 02 72 21 20 12 08 A3 4C 18 02 12 00 00 00 02 FF 34 00 00 05 FF 35 00 00 80 3F 02 FF 36 00 00 02 FF 37 00 00 02 FF 38 00 00 04 ED FF 39 80 20 01 01 05 FF 3A 00 00 C0 FF 06 FF 20 F4 0B 00 00 00 00 03 FF 21 03 00 00 03 FF 22 04 00 00 03 FF 23 0B 00 00 03 FF 24 3C 00 00 05 03 00 00 00 00 05 83 FF 09 00 00 00 00 85 40 03 00 00 00 00 85 40 83 FF 09 00 00 00 00 05 83 FF 0D 00 00 00 00 85 40 83 FF 0D 00 00 00 00 05 83 FF 01 00 00 00 00 05 83 FF 02 00 00 00 00 05 83 FF 03 00 00 00 00 05 FD 61 00 00 00 00 85 10 03 00 00 00 00 85 20 03 00 00 00 00 85 30 03 00 00 00 00 85 80 10 03 00 00 00 00 0F 48 16 
    */

    // DEV NOTE: the data sheet is a mess for all of these telegrams, but *especially* telegram 3.  Some of these might be total guesses
}
