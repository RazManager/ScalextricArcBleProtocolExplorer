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
        public const string adapterInterfaceName = "org.bluez.Adapter1";
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

                    var objectManager = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IObjectManager>(serviceName, Tmds.DBus.ObjectPath.Root);

                    var watchInterfacesAddedTask = objectManager.WatchInterfacesAddedAsync(
                        ((Tmds.DBus.ObjectPath objectPath, IDictionary<string, IDictionary<string, object>> interfaces) handler) =>
                            {
                                Console.WriteLine();
                                _logger.LogInformation($"{handler.objectPath} added...");
                                LogDBusObject(handler.objectPath, handler.interfaces);
                            },
                        exception =>
                        {
                            _logger.LogError(exception.Message);
                        }
                    );

                    var watchInterfacesRemovedTask = objectManager.WatchInterfacesRemovedAsync(
                        ((Tmds.DBus.ObjectPath objectPath, string[] interfaces) handler) =>
                        {
                            Console.WriteLine();
                            _logger.LogInformation($"{handler.objectPath} removed...");
                            Console.WriteLine("Interfaces:");
                            foreach (var iface in handler.interfaces)
                            {
                                Console.WriteLine(iface);
                            }
                        },
                        exception =>
                        {
                            _logger.LogError(exception.Message);
                        }
                    );

                    var adapter = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IAdapter1>(serviceName, adapterInterfaceName);
                    await adapter.StartDiscoveryAsync();

                    var dBusObjects = await objectManager.GetManagedObjectsAsync();
                    Console.WriteLine($"{dBusObjects.Count} objectPaths found.");
                    foreach (var dBusObject in dBusObjects)
                    {
                        LogDBusObject(dBusObject.Key, dBusObject.Value);
                    }

                    // Check that org.bluez.Adapter1 exists

                    var interfaces = dBusObjects.SelectMany(x => x.Value);
                    foreach (var item in interfaces)
                    {
                        Console.WriteLine(item.Value);
                    }


                    //var device = Connection.System.CreateProxy<IDevice1>(BluezConstants.DbusService, args.objectPath);

                    // Check if Scalextrixc ARC already found...

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("BluezMonitorService is running...");
                        await Task.Delay(10000, cancellationToken);
                    }

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


        private void LogDBusObject(Tmds.DBus.ObjectPath objectPath, IDictionary<string, IDictionary<string, object>> interfaces)
        {
            Console.WriteLine("===================================================================");
            Console.WriteLine($"objectPath={objectPath}");
            foreach (var iface in interfaces)
            {
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine($"interface={iface.Key}");
                if (iface.Value.Any())
                {
                    Console.WriteLine("Properties:");
                    foreach (var prop in iface.Value)
                    {
                        switch (prop.Value)
                        {
                            case string[]:
                                Console.WriteLine($"Values for property={prop.Key}:");
                                foreach (var item in (string[])prop.Value)
                                {
                                    Console.WriteLine(item);
                                }
                                break;

                            case Dictionary<ushort, object>:
                                Console.WriteLine($"Values for property={prop.Key}:");
                                foreach (var item in (Dictionary<ushort, object>)prop.Value)
                                {
                                    Console.WriteLine($"{item.Key}={item.Value}");
                                }
                                break;

                            case Dictionary<string, object>:
                                Console.WriteLine($"Values for property={prop.Key}:");
                                foreach (var item in (Dictionary<string, object>)prop.Value)
                                {
                                    Console.WriteLine($"{item.Key}={item.Value}");
                                }
                                break;

                            default:
                                Console.WriteLine($"{prop.Key}={prop.Value}");
                                break;
                        }
                    }
                }
            }
        }
    }
}
