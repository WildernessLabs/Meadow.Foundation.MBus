using Meadow.Devices;
using Meadow.Foundation.MBus.RelayMBus;
using Meadow.Hardware;
using System.Threading.Tasks;

namespace Meadow.Foundation.MBus.MBus_Feather_Sample;


public class MBM55R : MBusSerialServer
{
    public MBM55R(ISerialPort port)
        : base(port)
    {
        port.BaudRate = 9600;
        port.Parity = Parity.Even;
        port.Open();
    }
}

public class MeadowApp : App<F7FeatherV2>
{
    //<!=SNIP=>
    private PadPulsM2 pulseCounter;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        var port = Device.PlatformOS.GetSerialPortName("com1").CreateSerialPort(9600, parity: Parity.Even);

        var mbm = new MBM55R(port);
        pulseCounter = new PadPulsM2(mbm);
        pulseCounter.StartMonitoring();

        Resolver.Log.Info($"Counter ports: {pulseCounter.Ports[0].ID}  {pulseCounter.Ports[1].ID}");

        return base.Initialize();
    }

    public override async Task Run()
    {
        Resolver.Log.Info("Run");

        while (true)
        {
            Resolver.Log.Info($"now: {pulseCounter.Ports[0].CurrentDateTime}");
            await Task.Delay(2000);
        }

    }

    //<!=SNOP=>
}