using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Services
{
    public class BluezMonitorService : BackgroundService
    {
        private readonly ILogger<BluezMonitorService> _logger;

        public const string serviceName = "org.bluez";
        public const string AdapterInterface = "org.bluez.Adapter1";
        public const string DeviceInterface = "org.bluez.Device1";
        public const string GattServiceInterface = "org.bluez.GattService1";
        public const string GattCharacteristicInterface = "org.bluez.GattCharacteristic1";

        //[DBusInterface("org.freedesktop.DBus.ObjectManager")]
        //interface IObjectManager : IDBusObject

        //[DBusInterface("org.bluez.AgentManager1")]
        //interface IAgentManager1 : IDBusObject

        //[DBusInterface("org.bluez.ProfileManager1")]
        //interface IProfileManager1 : IDBusObject

        //[DBusInterface("org.bluez.Adapter1")]
        //interface IAdapter1 : IDBusObject

        //[DBusInterface("org.bluez.GattManager1")]
        //interface IGattManager1 : IDBusObject

        //[DBusInterface("org.bluez.LEAdvertisingManager1")]
        //interface ILEAdvertisingManager1 : IDBusObject

        //[DBusInterface("org.bluez.Media1")]
        //interface IMedia1 : IDBusObject

        //[DBusInterface("org.bluez.NetworkServer1")]
        //interface INetworkServer1 : IDBusObject


        public BluezMonitorService(ILogger<BluezMonitorService> logger)
        {
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine(Environment.OSVersion.Platform);

                    var services = await Tmds.DBus.Connection.System.ListServicesAsync();
                    Console.WriteLine($"Number of services: {services.Length}");
                    foreach (var service in services)
                    {
                        Console.WriteLine(service);
                    }

                    var activatableServices = await Tmds.DBus.Connection.System.ListActivatableServicesAsync();
                    Console.WriteLine($"Number of activatable services: {activatableServices.Length}");
                    foreach (var activatableService in activatableServices)
                    {
                        Console.WriteLine(activatableServices);
                    }

                    var x = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IObjectManager>(serviceName, GetType(bluez.DBus.IObjectManager.   "d");
                    x.


                    _logger.LogInformation("BluezMonitorService is running...");
                    await Task.Delay(10000, cancellationToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message);
                    await Task.Delay(10000, cancellationToken);
                }
            }
        }
    }
}
