using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

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

                    //var services = await Tmds.DBus.Connection.System.ListServicesAsync();
                    //Console.WriteLine($"Number of services: {services.Length}");
                    //foreach (var service in services)
                    //{
                    //    Console.WriteLine(service);
                    //}

                    var activatableServices = await Tmds.DBus.Connection.System.ListActivatableServicesAsync();
                    //Console.WriteLine($"Number of activatable services: {activatableServices.Length}");
                    //foreach (var activatableService in activatableServices)
                    //{
                    //    Console.WriteLine(activatableService);
                    //}

                    if (activatableServices.SingleOrDefault(x => x == serviceName) is null)
                    {
                        _logger.LogError($"{serviceName} is not an \"activateable\" D-Bus service. Please install bluez for the needed Bluetooth Low Energy functionality.");
                        return;
                    }

                    var isServiceActive = await Tmds.DBus.Connection.System.IsServiceActiveAsync(serviceName);
                    if (!isServiceActive)
                    {
                        _logger.LogInformation($"Starting {serviceName}.");
                        var serviceStartResult = await Tmds.DBus.Connection.System.ActivateServiceAsync(serviceName);
                        switch (serviceStartResult)
                        {
                            case Tmds.DBus.ServiceStartResult.Started:
                                _logger.LogInformation($"{serviceName} started.");
                                break;
                            case Tmds.DBus.ServiceStartResult.AlreadyRunning:
                                _logger.LogInformation($"{serviceName} was already running.");
                                break;
                        }
                    }

                    var objectManager = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IObjectManager>(serviceName, "/");

                    var watchInterfacesAddedTask = objectManager.WatchInterfacesAddedAsync(
                        ((Tmds.DBus.ObjectPath objectPath, IDictionary<string, IDictionary<string, object>> interfaces) handler) =>
                            {
                                _logger.LogInformation($"{handler.objectPath} added...");
                            },
                        exception =>
                        {
                            _logger.LogError(exception.Message);
                        }
                    );

                    var watchInterfacesRemovedTask = objectManager.WatchInterfacesRemovedAsync(
                        ((Tmds.DBus.ObjectPath objectPath, string[] interfaces) handler) =>
                        {
                            _logger.LogInformation($"{handler.objectPath} removed...");
                        },
                        exception =>
                        {
                            _logger.LogError(exception.Message);
                        }
                    );

                    var objectPaths = await objectManager.GetManagedObjectsAsync();
                    Console.WriteLine($"{objectPaths.Count} objectPaths found.");
                    foreach (var objectPath in objectPaths)
                    {
                        Console.WriteLine($"objectPath.Key={objectPath.Key}");
                        foreach (var iface in objectPath.Value)
                        {
                            Console.WriteLine($"iface.Key={iface.Key}");
                            foreach (var item in iface.Value)
                            {
                                Console.WriteLine($"item.Key={item.Key} item.Value={item.Value}");
                            }
                        }
                    }


                    _logger.LogInformation("BluezMonitorService is running...");
                    await Task.Delay(10000, cancellationToken);

                    watchInterfacesAddedTask.Dispose();
                    watchInterfacesRemovedTask.Dispose();

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
