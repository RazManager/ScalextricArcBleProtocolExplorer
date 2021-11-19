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
using static bluez.DBus.GattService1Extensions;


namespace ScalextricArcBleProtocolExplorer.Services
{
    public class BluezMonitorService : IHostedService
    {
        public const string bluezServiceName = "org.bluez";
        public const string bluezAdapterInterfaceName = "org.bluez.Adapter1";
        public const string bluezDeviceInterfaceName = "org.bluez.Device1";
        public const string bluezGattServiceInterface = "org.bluez.GattService1";
        public const string bluezGattCharacteristicInterface = "org.bluez.GattCharacteristic1";
        public const string bluezGattDescriptorInterface = "org.bluez.GattDescriptor1";

        private class BluezInterfaceMetadata
        {
            public string InterfaceName { get; init; } = null!;
            public string? DeviceName { get; init; }
        }

        private ConcurrentDictionary<Tmds.DBus.ObjectPath, IEnumerable<BluezInterfaceMetadata>> _bluezObjectPathInterfaces = new();

        private readonly ILogger<BluezMonitorService> _logger;

        //private Tmds.DBus.ObjectPath? bluezAdapterObjectPath = null;
        private Tmds.DBus.ObjectPath? scalextricArcObjectPath = null;


        public BluezMonitorService(ILogger<BluezMonitorService> logger)
        {
            _logger = logger;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
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
                _bluezObjectPathInterfaces = new();

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
                    Console.WriteLine($"Number of activatable services: {activatableServices.Length}");
                    foreach (var activatableService in activatableServices)
                    {
                        Console.WriteLine(activatableService);
                    }

                    if (activatableServices.SingleOrDefault(x => x == bluezServiceName) is null)
                    {
                        _logger.LogError($"{bluezServiceName} is not an \"activateable\" D-Bus service. Please install bluez for the needed Bluetooth Low Energy functionality, and then re-start this application.");
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

                    // Find all D-Bus objects and their interfaces
                    var objectManager = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IObjectManager>(bluezServiceName, Tmds.DBus.ObjectPath.Root);
                    var dBusObjects = await objectManager.GetManagedObjectsAsync();
                    foreach (var dBusObject in dBusObjects)
                    {
                        InterfaceAdded((dBusObject.Key, dBusObject.Value));
                    }

                    Console.WriteLine("bluezObjectPathInterfaces:");
                    foreach (var objectPathKp in _bluezObjectPathInterfaces)
                    {
                        Console.WriteLine(objectPathKp.Key);
                        foreach (var bluezInterfaceMetadata in objectPathKp.Value)
                        {
                            Console.WriteLine($"    {bluezInterfaceMetadata.InterfaceName} {bluezInterfaceMetadata.DeviceName}");
                        }
                    }

                    var bluezAdapterObjectPathKp = _bluezObjectPathInterfaces.SingleOrDefault(x => x.Value.Any(i => i.InterfaceName == bluezAdapterInterfaceName));
                    if (string.IsNullOrEmpty(bluezAdapterObjectPathKp.Key.ToString()))
                    {
                        _logger.LogError($"{bluezAdapterInterfaceName} does not exist. Please install bluez for the needed Bluetooth Low Energy functionality, and then re-start this application.");
                        return;
                    }

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

                    Console.WriteLine($"Creating bluez.DBus.IAdapter1 proxy: {bluezServiceName} {bluezAdapterObjectPathKp.Key}");
                    var bluezAdapterProxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IAdapter1>(bluezServiceName, bluezAdapterObjectPathKp.Key);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var scalextricArcObjectPathKps = _bluezObjectPathInterfaces.Where(x => x.Value.Any(i => i.InterfaceName == bluezDeviceInterfaceName && !string.IsNullOrEmpty(i.DeviceName) && i.DeviceName.Trim() == "Scalextric ARC"));

                        if (!scalextricArcObjectPathKps.Any())
                        {
                            if (scalextricArcObjectPath is not null)
                            {
                                // Disconnect...
                            }

                            if (await bluezAdapterProxy.GetDiscoveringAsync())
                            {
                                Console.WriteLine("Bluetooth device discovery already started.");
                            }
                            else
                            {
                                //adapter.SetDiscoveryFilterAsync
                                _logger.LogInformation("Starting Bluetooth device discovery.");
                                await bluezAdapterProxy.StartDiscoveryAsync();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Bluetooth device discovery not needed, device already found.");
                            if (await bluezAdapterProxy.GetDiscoveringAsync())
                            {
                                _logger.LogInformation("Stopping Bluetooth device discovery.");
                                await bluezAdapterProxy.StopDiscoveryAsync();
                            }

                            if (scalextricArcObjectPathKps.Count() >= 2)
                            {
                                _logger.LogInformation($"{scalextricArcObjectPathKps.Count()} Scalextric ARC powerbases found. No new connections will be attempted until there's only 1 available.");
                            }
                            else
                            {
                                await DevicesChangedAsync(scalextricArcObjectPathKps.First().Key);
                            }
                        }

                        _logger.LogInformation("BluezMonitorService is running...");

                        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
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

            if (args.interfaces.Keys.Any(x => x.StartsWith(bluezServiceName)))
            {
                var bluezInterfaceMetadatas = new List<BluezInterfaceMetadata>();
                foreach (var item in args.interfaces.Where(x => x.Key.StartsWith(bluezServiceName)))
                {
                    bluezInterfaceMetadatas.Add(new BluezInterfaceMetadata
                    {
                        InterfaceName = item.Key,
                        DeviceName = item.Value.SingleOrDefault(x => x.Key == "Name").Value?.ToString()?.Trim()
                    });
                }

                _bluezObjectPathInterfaces.TryAdd(args.objectPath, bluezInterfaceMetadatas);
            }
        }


        private void InterfaceRemoved((Tmds.DBus.ObjectPath objectPath, string[] interfaces) args)
        {
            Console.WriteLine();
            _logger.LogInformation($"{args.objectPath} removed...");
            _bluezObjectPathInterfaces.TryRemove(args.objectPath, out IEnumerable<BluezInterfaceMetadata>? bluezInterfaceMetadatas);
        }


        private async Task DevicesChangedAsync(Tmds.DBus.ObjectPath objectPath)
        {
            if (scalextricArcObjectPath is null)
            {
                try
                {
                    Console.WriteLine($"CreateProxy before CreateProxy<bluez.DBus.IDevice1>({bluezServiceName}, {objectPath}) ...");
                    var deviceProxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IDevice1>(bluezServiceName, objectPath);
                    Console.WriteLine($"CreateProxy after...");

                    if (await deviceProxy.GetConnectedAsync())
                    {
                        _logger.LogInformation("Scalextric ARC already connected.");
                        scalextricArcObjectPath = objectPath;
                    }
                    else
                    {
                        _logger.LogInformation("Connecting to Scalextric ARC.");
                        bool success = false;
                        for (int i = 1; i <= 5; i++)
                        {
                            try
                            {
                                Console.WriteLine($"ConnectAsync before {i} attempt(s).");
                                await deviceProxy.ConnectAsync();
                                Console.WriteLine($"ConnectAsync after {i} attempt(s).");
                                success = true;
                                break;
                            }
                            catch (Tmds.DBus.DBusException exception)
                            {
                                Console.WriteLine($"ConnectAsync exception: {exception.ErrorName}, {exception.ErrorMessage}");
                                await Task.Delay(TimeSpan.FromSeconds(5));
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                        if (success)
                        {
                            scalextricArcObjectPath = objectPath;
                        }
                    }

                    if (scalextricArcObjectPath != null)
                    {
                        if (!await deviceProxy.GetServicesResolvedAsync())
                        {
                            _logger.LogInformation("Waiting for Scalextric ARC services to be resolved.");
                            for (int i = 1; i <= 5; i++)
                            {
                                if (await deviceProxy.GetServicesResolvedAsync())
                                {
                                    break;
                                }

                                await Task.Delay(TimeSpan.FromSeconds(5));
                            }

                            if (!await deviceProxy.GetServicesResolvedAsync())
                            {
                                throw new Exception("Scalextric ARC services could not be resolved");
                            }
                        }

                        Console.WriteLine("Bluez objects and interfaces");
                        foreach (var item in _bluezObjectPathInterfaces)
                        {
                            Console.WriteLine($"{item.Key} {string.Join(", ", item.Value.Select(x => x.InterfaceName))}");
                        }

                        foreach (var item in _bluezObjectPathInterfaces.Where(x => x.Value.Any(i => i.InterfaceName == bluezGattServiceInterface)))
                        {
                            //Console.WriteLine();
                            Console.WriteLine($"GattService: {item.Key} {string.Join(", ", item.Value.Select(x => x.InterfaceName))}");
                            //Console.WriteLine($"Before CreateProxy<bluez.DBus.IGattService1>({bluezServiceName}, {item.Key})");
                            var proxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IGattService1>(bluezServiceName, item.Key);
                            //Console.WriteLine("After...");
                            var properties = await proxy.GetAllAsync();
                            //Console.WriteLine("After GetAllAsync...");
                            Console.WriteLine($"UUID={properties.UUID}");
                            //Console.WriteLine($"Device={gattService1Properties.Device}");
                            //Console.WriteLine($"Primary={gattService1Properties.Primary}");
                            //Console.WriteLine($"Includes={gattService1Properties.Includes}");
                        }

                        foreach (var item in _bluezObjectPathInterfaces.Where(x => x.Value.Any(i => i.InterfaceName == bluezGattCharacteristicInterface)))
                        {
                            Console.WriteLine();
                            Console.WriteLine($"GattCharacteristic: {item.Key} {string.Join(", ", item.Value.Select(x => x.InterfaceName))}");
                            //Console.WriteLine($"Before CreateProxy<bluez.DBus.IGattCharacteristic1>({bluezServiceName}, {item.Key})");
                            var proxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IGattCharacteristic1>(bluezServiceName, item.Key);
                            //Console.WriteLine("After...");
                            var properties = await proxy.GetAllAsync();
                            //Console.WriteLine("After GetAllAsync...");
                            Console.WriteLine($"UUID={properties.UUID}");
                            Console.WriteLine($"Service={properties.Service}");
                            Console.WriteLine($"Flags={string.Join(", ", properties.Flags)}");
                            //Console.WriteLine($"WriteAcquired={properties.WriteAcquired}");
                            //Console.WriteLine($"NotifyAcquired={properties.NotifyAcquired}");
                            //Console.WriteLine($"Notifying={properties.Notifying}");

                            if (properties.Flags.Contains("read"))
                            {
                                var value = await proxy.ReadValueAsync(new Dictionary<string, object>());
                                Console.WriteLine($"Length={value.Length}");
                                if (value.Length > 0)
                                {
                                    var valueUTF8 = System.Text.Encoding.UTF8.GetString(value);
                                    Console.WriteLine($"valueUTF8={valueUTF8}");
                                }
                            }
                        }

                        foreach (var item in _bluezObjectPathInterfaces.Where(x => x.Value.Any(i => i.InterfaceName == bluezGattDescriptorInterface)))
                        {
                            Console.WriteLine();
                            Console.WriteLine($"GattDescriptor: {item.Key} {string.Join(", ", item.Value.Select(x => x.InterfaceName))}");
                            //Console.WriteLine($"Before CreateProxy<bluez.DBus.IGattCharacteristic1>({bluezServiceName}, {item.Key})");
                            var proxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IGattDescriptor1>(bluezServiceName, item.Key);
                            //Console.WriteLine("After...");
                            var properties = await proxy.GetAllAsync();
                            //Console.WriteLine("After GetAllAsync...");
                            Console.WriteLine($"UUID={properties.UUID}");

                            if (properties.Value.Length > 0)
                            {
                                var valueASCII = System.Text.Encoding.ASCII.GetString(properties.Value);
                                var valueUTF8 = System.Text.Encoding.UTF8.GetString(properties.Value);
                                Console.WriteLine($"valueASCII={valueASCII}");
                                Console.WriteLine($"valueUTF8={valueUTF8}");
                            }
                            //Console.WriteLine($"WriteAcquired={gattCharacteristic1Properties.WriteAcquired}");
                            //Console.WriteLine($"NotifyAcquired={gattCharacteristic1Properties.NotifyAcquired}");
                            //Console.WriteLine($"Notifying={gattCharacteristic1Properties.Notifying}");
                        }

                        //var servicesUUID = await deviceProxy.GetUUIDsAsync();
                        //Console.WriteLine($"Device offers {servicesUUID.Length} service(s).");

                        //var deviceInfoServiceFound = servicesUUID.Any(uuid => String.Equals(uuid, "0000180a-0000-1000-8000-00805f9b34fb", StringComparison.OrdinalIgnoreCase));
                        //if (!deviceInfoServiceFound)
                        //{
                        //    Console.WriteLine("Device doesn't have the Device Information Service. Try pairing first?");
                        //    return;
                        //};

                        //var gProxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IGattService1>()

                        //var

                        /// Console.WriteLine("Retrieving Device Information service...");
                        //var service = await deviceProxy.ser .GetServiceDataAsync();
                        //Console.WriteLine("Device Information service retrieved...");
                        //foreach (var item in service)
                        //{
                        //    Console.WriteLine($"{item.Key}={item.Value}");
                        //}


                        //var modelNameCharacteristic = await service.GetCharacteristicAsync(GattConstants.ModelNameCharacteristicUUID);
                        //var manufacturerCharacteristic = await service.GetCharacteristicAsync(GattConstants.ManufacturerNameCharacteristicUUID);
                    }

                    Console.WriteLine("OK...");

                    //    await DeviceConnectedAndServicesResolvedAsync(deviceProxy);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.GetType().FullName);
                    _logger.LogError(exception.Message);
                }
            }
        }


        private Task DeviceConnectedAndServicesResolvedAsync(bluez.DBus.IDevice1 device)
        {
            var tcs = new TaskCompletionSource();

            Task<IDisposable> t;

            t = device.WatchPropertiesAsync(propertyChanges =>
            {
                var connected = propertyChanges.Get<string>("Connected");
                var servicesResolved = propertyChanges.Get<string>("ServicesResolved");

                Console.WriteLine($"connected={connected}");
                Console.WriteLine($"servicesResolved={servicesResolved}");
                //tcs.SetResult();
            });
            t.Dispose();

            return tcs.Task;
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
