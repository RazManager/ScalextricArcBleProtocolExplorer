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
using static bluez.DBus.GattCharacteristic1Extensions;


namespace ScalextricArcBleProtocolExplorer.Services
{
    public class BluezMonitorService : IHostedService
    {
        public const string bluezService = "org.bluez";
        public const string bluezAdapterInterface = "org.bluez.Adapter1";
        public const string bluezDeviceInterface = "org.bluez.Device1";
        //public const string bluezGattServiceInterface = "org.bluez.GattService1";
        public const string bluezGattCharacteristicInterface = "org.bluez.GattCharacteristic1";

        private class BluezInterfaceMetadata
        {
            public string BluezInterface { get; init; } = null!;
            public Guid? UUID { get; init; }
            public string? DeviceName { get; init; }
        }

        private ConcurrentDictionary<Tmds.DBus.ObjectPath, IEnumerable<BluezInterfaceMetadata>> _bluezObjectPathInterfaces = new();
        private Tmds.DBus.ObjectPath? scalextricArcObjectPath = null;
        private bluez.DBus.IDevice1? scalextricArcProxy = null;

        private readonly Guid manufacturerNameCharacteristicUuid = new Guid("00002a29-0000-1000-8000-00805f9b34fb");
        private readonly Guid modelNumberCharacteristicUuid = new Guid("00002a24-0000-1000-8000-00805f9b34fb");
        private readonly Guid hardwareRevisionCharacteristicUuid = new Guid("00002a27-0000-1000-8000-00805f9b34fb");
        private readonly Guid firmwareRevisionCharacteristicUuid = new Guid("00002a26-0000-1000-8000-00805f9b34fb");
        private readonly Guid softwareRevisionCharacteristicUuid = new Guid("00002a28-0000-1000-8000-00805f9b34fb");

        private readonly Guid throttleCharacteristicUuid = new Guid("00003b09-0000-1000-8000-00805f9b34fb");
        private bluez.DBus.IGattCharacteristic1? throttleCharacteristicProxy = null;
        private Task? throttleCharacteristicWatchTask = null;

        private readonly Guid slotCharacteristicUuid = new Guid("00003b0b-0000-1000-8000-00805f9b34fb");
        private bluez.DBus.IGattCharacteristic1? slotCharacteristicProxy = null;
        private Task? slotCharacteristicWatchTask = null;

        private readonly ScalextricArcState _scalextricArcState;
        private readonly ILogger<BluezMonitorService> _logger;


        public BluezMonitorService(ScalextricArcState scalextricArcState,
                                  ILogger<BluezMonitorService> logger)
        {
            _scalextricArcState = scalextricArcState;
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

            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                _logger.LogError("You need to be running Linux for this application.");
                return;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                _bluezObjectPathInterfaces = new();
                scalextricArcObjectPath = null;
                scalextricArcProxy = null;
                throttleCharacteristicWatchTask = null;
                slotCharacteristicWatchTask = null;

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

                    if (activatableServices.SingleOrDefault(x => x == bluezService) is null)
                    {
                        _logger.LogError($"{bluezService} is not an \"activateable\" D-Bus service. Please install bluez for the needed Bluetooth Low Energy functionality, and then re-start this application.");
                        return;
                    }

                    var isServiceActive = await Tmds.DBus.Connection.System.IsServiceActiveAsync(bluezService);
                    if (!isServiceActive)
                    {
                        _logger.LogInformation($"Starting {bluezService}.");
                        var serviceStartResult = await Tmds.DBus.Connection.System.ActivateServiceAsync(bluezService);
                        switch (serviceStartResult)
                        {
                            case Tmds.DBus.ServiceStartResult.Started:
                                _logger.LogInformation($"{bluezService} started.");
                                break;
                            case Tmds.DBus.ServiceStartResult.AlreadyRunning:
                                _logger.LogInformation($"{bluezService} was already running.");
                                break;
                        }
                    }

                    // Find all D-Bus objects and their interfaces
                    var objectManager = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IObjectManager>(bluezService, Tmds.DBus.ObjectPath.Root);
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
                            Console.WriteLine($"    {bluezInterfaceMetadata.BluezInterface} {bluezInterfaceMetadata.DeviceName}");
                        }
                    }

                    var bluezAdapterObjectPathKp = _bluezObjectPathInterfaces.SingleOrDefault(x => x.Value.Any(i => i.BluezInterface == bluezAdapterInterface));
                    if (string.IsNullOrEmpty(bluezAdapterObjectPathKp.Key.ToString()))
                    {
                        _logger.LogError($"{bluezAdapterInterface} does not exist. Please install bluez for the needed Bluetooth Low Energy functionality, and then re-start this application.");
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

                    Console.WriteLine($"Creating bluez.DBus.IAdapter1 proxy: {bluezService} {bluezAdapterObjectPathKp.Key}");
                    var bluezAdapterProxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IAdapter1>(bluezService, bluezAdapterObjectPathKp.Key);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var scalextricArcObjectPathKps = _bluezObjectPathInterfaces.Where(x => x.Value.Any(i => i.BluezInterface == bluezDeviceInterface && !string.IsNullOrEmpty(i.DeviceName) && i.DeviceName.Trim() == "Scalextric ARC"));

                        if (!scalextricArcObjectPathKps.Any())
                        {
                            if (throttleCharacteristicWatchTask is not null)
                            {
                                throttleCharacteristicWatchTask.Dispose();
                                throttleCharacteristicWatchTask = null;
                            }

                            if (throttleCharacteristicProxy is not null)
                            {
                                if (await throttleCharacteristicProxy.GetNotifyingAsync())
                                {
                                    await throttleCharacteristicProxy.StopNotifyAsync();
                                }
                                throttleCharacteristicProxy = null;
                            }

                            if (slotCharacteristicWatchTask is not null)
                            {
                                slotCharacteristicWatchTask.Dispose();
                                slotCharacteristicWatchTask = null;
                            }

                            if (slotCharacteristicProxy is not null)
                            {
                                if (await slotCharacteristicProxy.GetNotifyingAsync())
                                {
                                    await slotCharacteristicProxy.StopNotifyAsync();
                                }
                                slotCharacteristicProxy = null;
                            }

                            if (scalextricArcProxy is not null)
                            {
                                if (await scalextricArcProxy.GetConnectedAsync())
                                {
                                    await scalextricArcProxy.DisconnectAsync();
                                }
                            }

                            scalextricArcObjectPath = null;

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
                                await ScalextricArcChangedAsync(scalextricArcObjectPathKps.First().Key);
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
                }
                catch (Tmds.DBus.DBusException exception)
                {
                    switch (exception.ErrorName)
                    {
                        case "org.bluez.Error.NotReady":
                            _logger.LogError(exception.ErrorName);
                            break;

                        case "org.bluez.Error.Failed":
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
                }

                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
        }


        private void InterfaceAdded((Tmds.DBus.ObjectPath objectPath, IDictionary<string, IDictionary<string, object>> interfaces) args)
        {
            Console.WriteLine();
            Console.WriteLine($"{args.objectPath} added with the following interfaces...");
            foreach (var iface in args.interfaces)
            {
                Console.WriteLine(iface.Key);
            }

            if (args.interfaces.Keys.Any(x => x.StartsWith(bluezService)))
            {
                var bluezInterfaceMetadatas = new List<BluezInterfaceMetadata>();
                foreach (var item in args.interfaces.Where(x => x.Key.StartsWith(bluezService)))
                {
                    if (item.Key == bluezDeviceInterface)
                    {
                        LogDBusObject(args.objectPath, args.interfaces);
                    }

                    Guid? uuid = null;
                    var uuidStr = item.Value.SingleOrDefault(x => x.Key == "UUID").Value;
                    if (uuidStr is not null)
                    {
                        uuid = new Guid(uuidStr.ToString()!);
                    }
                    bluezInterfaceMetadatas.Add(new BluezInterfaceMetadata
                    {
                        BluezInterface = item.Key,
                        UUID = uuid,
                        DeviceName = item.Value.SingleOrDefault(x => item.Key == bluezDeviceInterface && x.Key == "Name").Value?.ToString()?.Trim()
                    });
                }

                _bluezObjectPathInterfaces.TryAdd(args.objectPath, bluezInterfaceMetadatas);
            }
        }


        private void InterfaceRemoved((Tmds.DBus.ObjectPath objectPath, string[] interfaces) args)
        {
            Console.WriteLine();
            Console.WriteLine($"{args.objectPath} removed...");
            _bluezObjectPathInterfaces.TryRemove(args.objectPath, out IEnumerable<BluezInterfaceMetadata>? bluezInterfaceMetadatas);
        }


        private async Task ScalextricArcChangedAsync(Tmds.DBus.ObjectPath objectPath)
        {
            if (scalextricArcObjectPath is null)
            {
                try
                {
                    Console.WriteLine($"CreateProxy before CreateProxy<bluez.DBus.IDevice1>({bluezService}, {objectPath}) ...");
                    scalextricArcProxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IDevice1>(bluezService, objectPath);
                    Console.WriteLine($"CreateProxy after...");

                    if (await scalextricArcProxy.GetConnectedAsync())
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
                                await scalextricArcProxy.ConnectAsync();
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
                        else
                        {
                            _logger.LogInformation("Could not connect to Scalextric ARC.");
                        }
                    }

                    if (scalextricArcObjectPath != null)
                    {
                        if (!await scalextricArcProxy.GetServicesResolvedAsync())
                        {
                            _logger.LogInformation("Waiting for Scalextric ARC services to be resolved.");
                            for (int i = 1; i <= 5; i++)
                            {
                                if (await scalextricArcProxy.GetServicesResolvedAsync())
                                {
                                    break;
                                }

                                await Task.Delay(TimeSpan.FromSeconds(5));
                            }

                            if (!await scalextricArcProxy.GetServicesResolvedAsync())
                            {
                                throw new Exception("Scalextric ARC services could not be resolved");
                            }
                        }

                        Console.WriteLine("Bluez objects and interfaces");
                        foreach (var item in _bluezObjectPathInterfaces)
                        {
                            Console.WriteLine($"{item.Key} {string.Join(", ", item.Value.Select(x => x.BluezInterface))}");
                        }

                        //foreach (var item in _bluezObjectPathInterfaces.Where(x => x.Value.Any(i => i.BluezInterface == bluezGattServiceInterface)).OrderBy(x => x.Key))
                        //{
                        //    Console.WriteLine();
                        //    Console.WriteLine($"GattService: {item.Key} {string.Join(", ", item.Value.Select(x => x.BluezInterface))}");
                        //    //Console.WriteLine($"Before CreateProxy<bluez.DBus.IGattService1>({bluezServiceName}, {item.Key})");
                        //    var proxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IGattService1>(bluezService, item.Key);
                        //    //Console.WriteLine("After...");
                        //    var properties = await proxy.GetAllAsync();
                        //    //Console.WriteLine("After GetAllAsync...");
                        //    Console.WriteLine($"UUID={properties.UUID}");
                        //    //Console.WriteLine($"Device={gattService1Properties.Device}");
                        //    //Console.WriteLine($"Primary={gattService1Properties.Primary}");
                        //    //Console.WriteLine($"Includes={gattService1Properties.Includes}");
                        //    Console.WriteLine($"Includes:");
                        //    foreach (var include in properties.Includes)
                        //    {
                        //        Console.WriteLine(include);
                        //    }
                        //}

                        string? manufacturerName = null;
                        string? modelNumber = null;
                        string? hardwareRevision = null;
                        string? firmwareRevision = null;
                        string? softwareRevision = null;
                        foreach (var item in _bluezObjectPathInterfaces.Where(x => x.Key.ToString().StartsWith(scalextricArcObjectPath.ToString()!) && x.Value.Any(i => i.BluezInterface == bluezGattCharacteristicInterface)).OrderBy(x => x.Key))
                        {
                            Console.WriteLine();
                            Console.WriteLine($"GattCharacteristic: {item.Key} {string.Join(", ", item.Value.Select(x => x.BluezInterface))}");
                            //Console.WriteLine($"Before CreateProxy<bluez.DBus.IGattCharacteristic1>({bluezServiceName}, {item.Key})");
                            var proxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IGattCharacteristic1>(bluezService, item.Key);
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

                                    if (new Guid(properties.UUID) == manufacturerNameCharacteristicUuid)
                                    {
                                        manufacturerName = valueUTF8;
                                    }
                                    else if (new Guid(properties.UUID) == modelNumberCharacteristicUuid)
                                    {
                                        modelNumber = valueUTF8;
                                    }
                                    else if (new Guid(properties.UUID) == hardwareRevisionCharacteristicUuid)
                                    {
                                        hardwareRevision = valueUTF8;
                                    }
                                    else if (new Guid(properties.UUID) == firmwareRevisionCharacteristicUuid)
                                    {
                                        firmwareRevision = valueUTF8;
                                    }
                                    else if (new Guid(properties.UUID) == softwareRevisionCharacteristicUuid)
                                    {
                                        softwareRevision = valueUTF8;
                                    }
                                }
                            }

                            if (new Guid(properties.UUID) == throttleCharacteristicUuid)
                            {
                                throttleCharacteristicProxy = proxy;
                                Console.WriteLine("StartNotifyAsync before");
                                await throttleCharacteristicProxy.StartNotifyAsync();
                                Console.WriteLine("StartNotifyAsync after");
                                throttleCharacteristicWatchTask = throttleCharacteristicProxy.WatchPropertiesAsync(throttleCharacteristicWatchProperties);
                            }

                            if (new Guid(properties.UUID) == slotCharacteristicUuid)
                            {
                                slotCharacteristicProxy = proxy;
                                Console.WriteLine("StartNotifyAsync before");
                                await slotCharacteristicProxy.StartNotifyAsync();
                                Console.WriteLine("StartNotifyAsync after");
                                slotCharacteristicWatchTask = slotCharacteristicProxy.WatchPropertiesAsync(slotCharacteristicWatchProperties);
                            }
                        }

                        _scalextricArcState.DeviceState.Set
                        (
                            manufacturerName,
                            modelNumber,
                            hardwareRevision,
                            firmwareRevision,
                            softwareRevision
                        );

                        //foreach (var item in _bluezObjectPathInterfaces.Where(x => x.Value.Any(i => i.InterfaceName == bluezGattDescriptorInterface)).OrderBy(x => x.Key))
                        //{
                        //    Console.WriteLine();
                        //    Console.WriteLine($"GattDescriptor: {item.Key} {string.Join(", ", item.Value.Select(x => x.InterfaceName))}");
                        //    //Console.WriteLine($"Before CreateProxy<bluez.DBus.IGattCharacteristic1>({bluezServiceName}, {item.Key})");
                        //    var proxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IGattDescriptor1>(bluezServiceName, item.Key);
                        //    //Console.WriteLine("After...");
                        //    var properties = await proxy.GetAllAsync();
                        //    //Console.WriteLine("After GetAllAsync...");
                        //    Console.WriteLine($"UUID={properties.UUID}");
                        //    Console.WriteLine($"Characteristic={properties.Characteristic}");

                        //    if (properties.Value.Length > 0)
                        //    {
                        //        var valueASCII = System.Text.Encoding.ASCII.GetString(properties.Value);
                        //        var valueUTF8 = System.Text.Encoding.UTF8.GetString(properties.Value);
                        //        Console.WriteLine($"valueASCII={valueASCII}");
                        //        Console.WriteLine($"valueUTF8={valueUTF8}");
                        //    }
                        //}
                    }

                    Console.WriteLine("OK...");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.GetType().FullName);
                    _logger.LogError(exception.Message);
                }
            }
        }


        public void throttleCharacteristicWatchProperties(Tmds.DBus.PropertyChanges propertyChanges)
        {
            foreach (var item in propertyChanges.Changed)
            {
                if (item.Key == "Value")
                {
                    var value = (byte[])item.Value;
                    Console.WriteLine($"PS={value[0]}, 1={value[1]}, 2={value[2]}, 3={value[3]}, 4={value[4]}, 5={value[5]}, 6={value[6]}, AD={value[11] & 0b1}");
                    Console.WriteLine($"brake1={(value[1] & 0b1000000) > 0}, LC1={(value[1] & 0b10000000) > 0}, LC1D={(value[11] & 0b100) > 0}");
                    Console.WriteLine($"ctrlVersion1={value[14]}, ctrlVersion2={value[15]}, ctrlVersion3={value[16]}, ctrlVersion4={value[17]}, ctrlVersion5={value[18]}, ctrlVersion6={value[19]}");
                    Console.WriteLine($"timestamp1={value[7]}, timestamp2={value[8]}, timestamp3={value[9]}, timestamp14={value[10]}, timestamp={(value[7] + value[8] * 2 ^ 8 + value[9] * 2 ^ 16 + value[10] * 2 ^ 24)}");

                    _scalextricArcState.ThrottleState.Set
                    (
                        value[0],
                        value[12],
                        value[13],
                        (value[11] & 0b1) > 0,
                        (byte)(value[1] & 0b111111),
                        (value[1] & 0b1000000) > 0,
                        (value[1] & 0b10000000) > 0,
                        (value[11] & 0b100) > 0,
                        value[14],
                        (byte)(value[2] & 0b111111),
                        (value[2] & 0b1000000) > 0,
                        (value[2] & 0b10000000) > 0,
                        (value[11] & 0b1000) > 0,
                        value[15],
                        (byte)(value[3] & 0b111111),
                        (value[3] & 0b1000000) > 0,
                        (value[3] & 0b10000000) > 0,
                        (value[11] & 0b10000) > 0,
                        value[16],
                        (byte)(value[4] & 0b111111),
                        (value[4] & 0b1000000) > 0,
                        (value[4] & 0b10000000) > 0,
                        (value[11] & 0b100000) > 0,
                        value[17],
                        (byte)(value[5] & 0b111111),
                        (value[5] & 0b1000000) > 0,
                        (value[5] & 0b10000000) > 0,
                        (value[11] & 0b1000000) > 0,
                        value[18],
                        (byte)(value[6] & 0b111111),
                        (value[6] & 0b1000000) > 0,
                        (value[6] & 0b10000000) > 0,
                        (value[11] & 0b10000000) > 0,
                        value[19],
                        (uint)(value[7] + value[8] * 2 ^ 8 + value[9] * 2 ^ 16 + value[10] * 2 ^ 24)
                    );
                }
            }
            //Console.WriteLine("propertyChanges.Invalidated:");
            //foreach (var item in propertyChanges.Invalidated)
            //{
            //    Console.WriteLine(item);
            //}
        }


        public void slotCharacteristicWatchProperties(Tmds.DBus.PropertyChanges propertyChanges)
        {
            foreach (var item in propertyChanges.Changed)
            {
                if (item.Key == "Value")
                {
                    var value = (byte[])item.Value;
                    Console.WriteLine($"PS={value[0]}, ID={value[1]}");

                    _scalextricArcState.SlotStates[value[1] - 1].Set
                    (
                        value[0],
                        value[1],
                        (uint)(value[2] + value[3] * 2 ^ 8 + value[4] * 2 ^ 16 + value[5] * 2 ^ 24),
                        (uint)(value[6] + value[7] * 2 ^ 8 + value[8] * 2 ^ 16 + value[9] * 2 ^ 24),
                        (uint)(value[10] + value[11] * 2 ^ 8 + value[12] * 2 ^ 16 + value[13] * 2 ^ 24),
                        (uint)(value[14] + value[15] * 2 ^ 8 + value[16] * 2 ^ 16 + value[17] * 2 ^ 24)
                    );
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
