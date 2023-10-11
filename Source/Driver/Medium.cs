namespace Meadow.MBus;

/// <summary>
/// The types of metered mediums for M-Bus devices
/// </summary>
public enum Medium : byte
{
    Other = 0,
    Oil = 1,
    Electricity = 2,
    Gas = 3,
    Heat = 4,
    Steam = 5,
    HotWater = 6,
    Water = 7,
    HeatCostAllocation = 8,
    CompressedAir = 9,
    CoolingLoadOut = 10,
    CoolingLoadIn = 11,
    HeatIn = 12,
    HeetCoolLoad = 13,
    BusSystem = 14,
    Unknown = 15
}
