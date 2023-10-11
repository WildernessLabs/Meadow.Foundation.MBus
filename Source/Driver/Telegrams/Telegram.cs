using System;

namespace Meadow.MBus;

/// <summary>
/// Represents a telegram used in MBus communication.
/// </summary>
public abstract record Telegram : ITelegram
{
    /// <summary>
    /// Gets the byte array representing the telegram data.
    /// </summary>
    protected byte[] Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Telegram"/> abstract record with the specified data.
    /// </summary>
    /// <param name="data">The data representing the telegram.</param>
    protected Telegram(byte[] data)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Telegram"/> abstract record with a specified size.
    /// </summary>
    /// <param name="size">The size of the telegram.</param>
    protected Telegram(int size)
    {
        Data = new byte[size];
    }


    /// <summary>
    /// Serializes the telegram and returns the byte array representing the data.
    /// </summary>
    /// <returns>The byte array representing the telegram data.</returns>
    public byte[] Serialize()
    {
        return Data;
    }

    /// <summary>
    /// Calculates the checksum for the specified portion of the data array.
    /// </summary>
    /// <param name="data">The data array.</param>
    /// <param name="start">The start index of the portion for calculating the checksum.</param>
    /// <param name="length">The length of the portion for calculating the checksum.</param>
    /// <returns>The calculated checksum byte.</returns>
    protected byte CalculateChecksum(byte[] data, int start, int length)
    {
        byte checksum = 0;

        for (var i = start; i < start + length; i++)
        {
            checksum += data[i];
        }

        return checksum;
    }

    /// <summary>
    /// Decodes the specified MBus record into the appropriate data type.
    /// </summary>
    /// <param name="record">The MBus record to decode.</param>
    /// <returns>The decoded data as an object.</returns>
    public static object? Decode(byte[] record)
    {
        return Decode(record[0], record[1], record[2..]);
    }

    /// <summary>
    /// Decodes the data based on the DIF and VIF values, returning the decoded object.
    /// </summary>
    /// <param name="dif">The DIF (Data Information Field) value.</param>
    /// <param name="vif">The VIF (Value Information Field) value.</param>
    /// <param name="data">The data array to decode.</param>
    /// <returns>The decoded object based on the DIF, VIF, and data.</returns>
    public static object? Decode(byte dif, byte vif, byte[] data)
    {
        // only the lower nibble matters
        switch (dif & 0xf)
        {
            case 0x00: // no data
                return 0;
            case 0x01: // byte
                return data[0];
            case 0x02: // short
                switch (vif)
                {
                    case 0x6c: // date
                        return DecodeDateTime(data, 2);
                    default:
                        return (short)DecodeInteger(data, 2);
                }
            case 0x03: //24-bit, use an int
                return (int)DecodeInteger(data, 3);
            case 0x04:
                switch (vif)
                {
                    case 0x6d:
                        // TODO: extended VIF
                        return DecodeDateTime(data, 4);
                    default:
                        return (int)DecodeInteger(data, 4);

                }
            case 0x05: // 4-byte single/float
                throw new NotImplementedException();
            case 0x06: // 24-bit
                switch (vif)
                {
                    case 0x6d:
                        // TODO: extended VIF
                        return DecodeDateTime(data, 6);
                    default:
                        return DecodeInteger(data, 6);

                }
            case 0x07: // 64-bit long
                return DecodeInteger(data, 8);
            case 0x09: // 2-digit BCD
                return (byte)DecodeBcd(data, 2);
            case 0x0a: // 4-digit BCD
                return (short)DecodeBcd(data, 4);
            case 0x0b: // 6-digit BCD
                return (int)DecodeBcd(data, 6);
            case 0x0c: // 8-digit BCD
                return (int)DecodeBcd(data, 8);
            case 0x0e: // 12-digit BCD
                return DecodeBcd(data, 8);
            default:
                // TODO:
                throw new NotImplementedException();

        }
    }

    private static long DecodeBcd(byte[] data, int length)
    {
        long val = 0;

        for (var i = length; i > 0; i--)
        {
            val = (val << 8) | data[i - 1];
        }

        return val;
    }

    private static DateTime? DecodeDateTime(byte[] data, int length)
    {
        var valid = false;
        for (var i = 0; i < length; i++)
        {
            if (data[i] != 0)
            {
                valid = true;
                break;
            }
        }

        if (!valid) return null;

        switch (length)
        {
            case 6: // Type I = Compound CP48: Date and Time
                return new DateTime(
                    2000 + (((data[3] & 0xE0) >> 5) | ((data[4] & 0xF0) >> 1)), // year
                    (data[4] & 0x0F), // month
                    data[3] & 0x1F, // day
                    data[2] & 0x1F, // hour
                    data[1] & 0x3F, // minute
                    data[0] & 0x3F  // second
                    );
            case 4: // Type F = Compound CP32: Date and Time
                return new DateTime(
                    2000 + (((data[2] & 0xE0) >> 5) | ((data[3] & 0xF0) >> 1)), // year
                    (data[3] & 0x0F), // month
                    data[2] & 0x1F, // day
                    data[1] & 0x1F, // hour
                    data[0] & 0x3F, // minute
                    0                 // second
                    );
            case 2: // Type G: Compound CP16: Date
                return new DateTime(
                    2000 + (((data[0] & 0xE0) >> 5) | ((data[1] & 0xF0) >> 1)), // year
                    (data[1] & 0x0F), // month
                    data[0] & 0x1F
                    );
            default: throw new ArgumentException();
        }
    }

    private static long DecodeInteger(byte[] data, int length)
    {
        long value = 0;

        var isNegative = (data[length - 1] & 0x80) != 0x00;

        for (var i = length; i > 0; i--)
        {
            if (isNegative)
            {
                value = (value << 8) + (data[i - 1] ^ 0xFF);
            }
            else
            {
                value = (value << 8) + data[i - 1];
            }
        }

        if (isNegative)
        {
            value = value * -1 - 1;
        }

        return value;
    }
}

