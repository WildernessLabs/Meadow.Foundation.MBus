using System;

namespace Meadow.Foundation.MBus.RelayMBus;

public partial class PadPulsM2
{
    /// <summary>
    /// Represents a port on the PadPuls M2 device.
    /// </summary>
    public class Port
    {
        internal PortIdentifier PortIdentifier { get; }

        /// <summary>
        /// Gets the unique ID associated with this port.
        /// </summary>
        public uint ID { get; private set; }

        /// <summary>
        /// Gets the current count for this port.
        /// </summary>
        public int CurrentCount { get; private set; }

        /// <summary>
        /// Gets the current date and time for this port.
        /// </summary>
        public DateTime CurrentDateTime { get; private set; }

        /// <summary>
        /// Gets the transmission counter for this port.
        /// </summary>
        public byte TransmissionCounter { get; private set; }

        /// <summary>
        /// Gets a value indicating whether Tariff A is enabled for this port.
        /// </summary>
        public bool TariffAEnabled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether long pulse sampling is enabled for this port.
        /// </summary>
        public bool LongPulseSamplingEnabled { get; private set; }

        /// <summary>
        /// Gets the count multiplier for this port.
        /// </summary>
        public float CountMultiplier { get; private set; }

        /// <summary>
        /// Gets the medium associated with this port.
        /// </summary>
        public Medium Medium { get; private set; }

        /// <summary>
        /// Gets the revision for this port.
        /// </summary>
        public byte Revision { get; private set; }

        internal Port(PortIdentifier portIdentifier)
        {
            PortIdentifier = portIdentifier;
        }

        internal void Update(byte[] userData)
        {
            // |  header           |   ID      |manuf|           | sig | count           | date            | last due  | due date count  | next due     |
            // 0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52
            // 68 2F 2F 68 08 00 72 02 34 79 18 AC 48 42 00 02 00 00 00 0C 00 00 00 00 00 04 6D 00 00 E5 27 42 6C 00 00 4C 00 00 00 00 00 42 EC 7E 01 31 0F 41 01 01 00 2C 16 
            // 68-2F-2F-68-08-00-72-01-34-79-18-AC-48-42-00-13-00-00-00-0C-00-00-00-00-00-04-6D-0E-03-E5-27-42-6C-00-00-4C-00-00-00-00-00-42-EC-7E-01-31-0F-40-01-01-00-4C-16
            ID = (uint)(userData[7] | userData[8] << 8 | userData[9] << 16 | userData[10] << 24);
            var manuf = userData[11] | userData[12] << 8;
            Revision = userData[13];
            TransmissionCounter = userData[15];
            Medium = (Medium)userData[16];
            CurrentCount = (int)Telegram.Decode(userData[19..]);
            CurrentDateTime = (DateTime)Telegram.Decode(userData[25..]);
            var lastDue = Telegram.Decode(userData[31..]);
            var dueDateCounter = Telegram.Decode(userData[35..]);
            var nextDueDate = Telegram.Decode(userData[41..]);
            TariffAEnabled = (userData[47] & (1 << 4)) != 0;
            LongPulseSamplingEnabled = (userData[47] & (1 << 6)) != 0; // 0 == 0.5ms, 1 == 1.5ms
            var numerator = userData[48];
            var denominator = userData[49];
            CountMultiplier = numerator / (float)denominator;
        }
    }
}
