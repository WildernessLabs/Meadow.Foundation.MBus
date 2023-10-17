using Meadow;
using Meadow.Foundation.MBus;
using Meadow.Foundation.MBus.SchneiderElectric;

internal class Program
{
    private static void Main(string[] args)
    {
        //        var port = new WindowsSerialPort("COM9", 2400);
        var port = new WindowsSerialPort("COM9", 9600, 8, Meadow.Hardware.Parity.Even);

        var server = new MBusSerialServer(port);

        var reader = new IEM3135(server, 2);
        reader.Refresh();
        while (true)
        {
            Thread.Sleep(1000);
        }
    }
}