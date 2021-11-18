using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using static bluez.DBus.Adapter1Extensions;
using static bluez.DBus.Device1Extensions;

namespace ScalextricArcBleProtocolExplorer.Services
{
    public class BluezMonitorService : IHostedService
    {
        private class DeviceInterfaceMetadata
        {
            public string? InterfaceName { get; init; }
            public string? DeviceName { get; init; }
            public bool Connected { get; set; } = false;
        }

        private readonly ILogger<BluezMonitorService> _logger;

        public const string bluezServiceName = "org.bluez";
        public const string bluezAdapterInterfaceName = "org.bluez.Adapter1";
        public const string bluezDeviceInterfaceName = "org.bluez.Device1";
        //public const string GattServiceInterface = "org.bluez.GattService1";
        //public const string GattCharacteristicInterface = "org.bluez.GattCharacteristic1";

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

        private ConcurrentDictionary<Tmds.DBus.ObjectPath, DeviceInterfaceMetadata> _devices = new ();


        public BluezMonitorService(ILogger<BluezMonitorService> logger)
        {
            _logger = logger;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine(Environment.OSVersion.Platform);
            _ = BluezDiscoveryAsync(cancellationToken);
            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        private async Task BluezDiscoveryAsync(CancellationToken cancellationToken)
        {
            Task? watchInterfacesAddedTask = null;
            Task? watchInterfacesRemovedTask = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    //var services = await Tmds.DBus.Connection.System.ListServicesAsync();
                    //Console.WriteLine($"Number of services: {services.Length}");
                    //foreach (var service in services)
                    //{
                    //    Console.WriteLine(service);
                    //}

                    var activatableServices = await Tmds.DBus.Connection.System.ListActivatableServicesAsync();
                    Console.WriteLine($"Number of activatable services: {activatableServices.Length}");
                    foreach (var activatableService in activatableServices)
                    {
                        Console.WriteLine(activatableService);
                    }

                    if (activatableServices.SingleOrDefault(x => x == bluezServiceName) is null)
                    {
                        _logger.LogError($"{bluezServiceName} is not an \"activateable\" D-Bus service. Please install bluez for the needed Bluetooth Low Energy functionality.");
                        return;
                    }

                    var isServiceActive = await Tmds.DBus.Connection.System.IsServiceActiveAsync(bluezServiceName);
                    if (!isServiceActive)
                    {
                        _logger.LogInformation($"Starting {bluezServiceName}.");
                        var serviceStartResult = await Tmds.DBus.Connection.System.ActivateServiceAsync(bluezServiceName);
                        switch (serviceStartResult)
                        {
                            case Tmds.DBus.ServiceStartResult.Started:
                                _logger.LogInformation($"{bluezServiceName} started.");
                                break;
                            case Tmds.DBus.ServiceStartResult.AlreadyRunning:
                                _logger.LogInformation($"{bluezServiceName} was already running.");
                                break;
                        }
                    }

                    var objectManager = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IObjectManager>(bluezServiceName, Tmds.DBus.ObjectPath.Root);

                    // Find all D-Bus objects
                    var dBusObjects = await objectManager.GetManagedObjectsAsync();
                    //Console.WriteLine($"{dBusObjects.Count} objectPaths found.");
                    //foreach (var dBusObject in dBusObjects)
                    //{
                    //    LogDBusObject(dBusObject.Key, dBusObject.Value);
                    //}

                    // Find all the interfaces for the D-Bus objects
                    var objectPathInterfaces = dBusObjects.SelectMany(objectPath => objectPath.Value, (objectPath, iface) => new { objectPath, iface });
                    //foreach (var item in objectPathInterfaces)
                    //{
                    //    Console.WriteLine(item.iface.Key);
                    //}

                    var bluezAdapter = objectPathInterfaces.SingleOrDefault(x => x.iface.Key == bluezAdapterInterfaceName);
                    if (bluezAdapter is null)
                    {
                        _logger.LogError($"{bluezAdapterInterfaceName} does not exist.");
                    }
                    else
                    {
                        if (watchInterfacesAddedTask is not null)
                        {
                            watchInterfacesAddedTask.Dispose();
                        }
                        watchInterfacesAddedTask = objectManager.WatchInterfacesAddedAsync(
                            InterfaceAdded,
                            exception =>
                            {
                                _logger.LogError(exception.Message);
                            }
                        );

                        if (watchInterfacesRemovedTask is not null)
                        {
                            watchInterfacesRemovedTask.Dispose();
                        }
                        watchInterfacesRemovedTask = objectManager.WatchInterfacesRemovedAsync(
                            InterfaceRemoved,
                            exception =>
                            {
                                _logger.LogError(exception.Message);
                            }
                        );

                        foreach (var dBusObject in dBusObjects)
                        {
                            Console.WriteLine($"{bluezAdapter.objectPath.Key} already discovered.");
                            InterfaceAdded((dBusObject.Key, dBusObject.Value));
                            //LogDBusObject(dBusObject.Key, dBusObject.Value);
                        }

                        Console.WriteLine($"Creating bluez.DBus.IAdapter1 proxy: {bluezServiceName} {bluezAdapter.objectPath.Key}");
                        var bluezAdapterProxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IAdapter1>(bluezServiceName, bluezAdapter.objectPath.Key);
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            if (_devices.Any())
                            {
                                _logger.LogInformation("Bluetooth device discovery not needed, device already found.");
                                if (await bluezAdapterProxy.GetDiscoveringAsync())
                                {
                                    _logger.LogInformation("Stopping Bluetooth device discovery.");
                                    await bluezAdapterProxy.StopDiscoveryAsync();
                                }
                            }
                            else
                            {
                                if (await bluezAdapterProxy.GetDiscoveringAsync())
                                {
                                    _logger.LogInformation("Bluetooth device discovery already started.");
                                }
                                else
                                {
                                    //adapter.SetDiscoveryFilterAsync
                                    _logger.LogInformation("Starting Bluetooth device discovery.");
                                    await bluezAdapterProxy.StartDiscoveryAsync();
                                }
                            }

                            _logger.LogInformation("BluezMonitorService is running...");

                            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

                            //await DevicesChangedAsync();
                        }
                    }

                    if (watchInterfacesAddedTask is not null)
                    {
                        watchInterfacesAddedTask.Dispose();
                    }
                    if (watchInterfacesRemovedTask is not null)
                    {
                        watchInterfacesRemovedTask.Dispose();
                    }

                    _logger.LogWarning("While loop end...");
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
                catch (Tmds.DBus.DBusException exception)
                {
                    switch (exception.ErrorName)
                    {
                        case "org.bluez.Error.NotReady":
                            _logger.LogError(exception.ErrorName);
                            break;

                        default:
                            _logger.LogError(exception.Message);
                            break;
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.GetType().FullName);
                    _logger.LogError(exception.Message);
                    await Task.Delay(10000, cancellationToken);
                }
            }
        }


        private void InterfaceAdded((Tmds.DBus.ObjectPath objectPath, IDictionary<string, IDictionary<string, object>> interfaces) args)
        {
            Console.WriteLine();
            _logger.LogInformation($"{args.objectPath} added with the following interfaces...");
            foreach (var iface in args.interfaces)
            {
                Console.WriteLine(iface.Key);
            }

            foreach (var iface in args.interfaces.Where(x => x.Key == bluezDeviceInterfaceName))
            {
                Console.WriteLine($"Checking {iface.Key} {iface.Value}");
                if (iface.Value.TryGetValue("Name", out object? value))
                {
                    var deviceName = value as string;
                    if (!string.IsNullOrEmpty(deviceName) && deviceName.Trim() == "Scalextric ARC")
                    {
                        LogDBusObject(args.objectPath, args.interfaces);

                        var deviceInterfaceMetadata = new DeviceInterfaceMetadata
                        {
                            InterfaceName = iface.Key,
                            DeviceName = deviceName
                        };
                        _devices.AddOrUpdate(args.objectPath, deviceInterfaceMetadata, (objPath, metadata) => deviceInterfaceMetadata);

                        _ = Task.Run(async () =>
                        {
                            _logger.LogInformation($"Scalextric ARC found. Waiting 10 seconds before attempting to connect to it in order for other devices to be found.");
                            await Task.Delay(TimeSpan.FromSeconds(10));
                            await DevicesChangedAsync();
                        });
                    }
                }
            }
            if (_devices.Any())
            {
                LogDBusObject(args.objectPath, args.interfaces);
            }
        }


        private void InterfaceRemoved((Tmds.DBus.ObjectPath objectPath, string[] interfaces) args)
        {
            Console.WriteLine();
            _logger.LogInformation($"{args.objectPath} removed...");
            _devices.TryRemove(args.objectPath, out DeviceInterfaceMetadata? value);
            _ = DevicesChangedAsync();
        }


        private async Task DevicesChangedAsync()
        {
            Console.WriteLine();
            Console.WriteLine($"DevicesChanged. Current device count = {_devices.Count}");

            if (_devices.Any(x => x.Value.Connected))
            {
                _logger.LogWarning($"At least one Scalextric ARC already connected, not trying to connect again...");
            }
            else
            {
                foreach (var device in _devices)
                {
                    try
                    {
                        Console.WriteLine($"{device.Key}, {device.Value.InterfaceName}, {device.Value.DeviceName}, {device.Value.Connected}");
                        Console.WriteLine($"CreateProxy before CreateProxy<bluez.DBus.IDevice1>({bluezServiceName}, {device.Key}) ...");
                        var deviceProxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IDevice1>(bluezServiceName, device.Key);
                        Console.WriteLine($"CreateProxy after...");

                        if (await deviceProxy.GetConnectedAsync())
                        {
                            _logger.LogInformation("Scalextric ARC already connected.");
                        }
                        else
                        {
                            Console.WriteLine($"ConnectAsync before..");
                            await deviceProxy.ConnectAsync();
                            Console.WriteLine($"ConnectAsync after..");
                            device.Value.Connected = true;

                            //var servicesUUID = await deviceProxy.GetUUIDsAsync();
                            //Console.WriteLine($"Device offers {servicesUUID.Length} service(s).");
 
                            //var deviceInfoServiceFound = servicesUUID.Any(uuid => String.Equals(uuid, "0000180a-0000-1000-8000-00805f9b34fb", StringComparison.OrdinalIgnoreCase));
                            //if (!deviceInfoServiceFound)
                            //{
                            //    Console.WriteLine("Device doesn't have the Device Information Service. Try pairing first?");
                            //    return;
                            //}

                            //Console.WriteLine("Retrieving Device Information service...");
                            //var service = await deviceProxy.GetServiceDataAsync();
                            //Console.WriteLine("Device Information service retrieved...");
                            //foreach (var item in service)
                            //{
                            //    Console.WriteLine($"{item.Key}={item.Value}");
                            //}





                            //var modelNameCharacteristic = await service.GetCharacteristicAsync(GattConstants.ModelNameCharacteristicUUID);
                            //var manufacturerCharacteristic = await service.GetCharacteristicAsync(GattConstants.ManufacturerNameCharacteristicUUID);


                            Console.WriteLine("OK...");


                        //    await DeviceConnectedAndServicesResolvedAsync(deviceProxy);
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception.GetType().FullName);
                        _logger.LogError(exception.Message);
                    }
                }
            }
        }


        private Task DeviceConnectedAndServicesResolvedAsync(bluez.DBus.IDevice1 device)
        {
            var tcs = new TaskCompletionSource();

            var t = device.WatchPropertiesAsync(propertyChanges =>
            {
                var connected = propertyChanges.Get<string>("Connected");
                var servicesResolved = propertyChanges.Get<string>("ServicesResolved");

                Console.WriteLine($"connected={connected}");
                Console.WriteLine($"servicesResolved={servicesResolved}");
                //tcs.SetResult();
                //t.Dispose();
            });

            return tcs.Task;
        }


        //private async Task ScalextricArcAsync(CancellationToken cancellationToken)
        //{

        //}




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
