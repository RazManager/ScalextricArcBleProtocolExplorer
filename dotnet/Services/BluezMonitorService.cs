using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using static bluez.DBus.Adapter1Extensions;
using static bluez.DBus.Device1Extensions;
using static bluez.DBus.GattCharacteristic1Extensions;
using static ScalextricArcBleProtocolExplorer.Services.ConnectionState;


namespace ScalextricArcBleProtocolExplorer.Services
{
    public class BluezMonitorService : IHostedService
    {
        public const string bluezService = "org.bluez";
        public const string bluezAdapterInterface = "org.bluez.Adapter1";
        public const string bluezDeviceInterface = "org.bluez.Device1";
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

        private const string manufacturerNameCharacteristicUuid = "00002a29-0000-1000-8000-00805f9b34fb";
        private const string modelNumberCharacteristicUuid = "00002a24-0000-1000-8000-00805f9b34fb";
        private const string hardwareRevisionCharacteristicUuid = "00002a27-0000-1000-8000-00805f9b34fb";
        private const string firmwareRevisionCharacteristicUuid = "00002a26-0000-1000-8000-00805f9b34fb";
        private const string softwareRevisionCharacteristicUuid = "00002a28-0000-1000-8000-00805f9b34fb";
        private const string dfuControlPointCharacteristicUuid = "00001531-1212-efde-1523-785feabcd123";
        private const string dfuPacketCharacteristicUuid = "00001532-1212-efde-1523-785feabcd123";
        private const string dfuRevisionCharacteristicUuid = "00001534-1212-efde-1523-785feabcd123";

        private const string commandCharacteristicUuid = "00003b0a-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _commandCharacteristicProxy = null;

        private const string slotCharacteristicUuid = "00003b0b-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _slotCharacteristicProxy = null;
        private Task? _slotCharacteristicWatchTask = null;

        private const string throttleCharacteristicUuid = "00003b09-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _throttleCharacteristicProxy = null;
        private Task? _throttleCharacteristicWatchTask = null;

        private const string throttleProfile1CharacteristicUuid = "0000ff01-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _throttleProfile1CharacteristicProxy = null;
        private Task? _throttleProfile1CharacteristicWatchTask = null;

        private const string throttleProfile2CharacteristicUuid = "0000ff02-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _throttleProfile2CharacteristicProxy = null;
        private Task? _throttleProfile2CharacteristicWatchTask = null;

        private const string throttleProfile3CharacteristicUuid = "0000ff03-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _throttleProfile3CharacteristicProxy = null;
        private Task? _throttleProfile3CharacteristicWatchTask = null;

        private const string throttleProfile4CharacteristicUuid = "0000ff04-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _throttleProfile4CharacteristicProxy = null;
        private Task? _throttleProfile4CharacteristicWatchTask = null;

        private const string throttleProfile5CharacteristicUuid = "0000ff05-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _throttleProfile5CharacteristicProxy = null;
        private Task? _throttleProfile5CharacteristicWatchTask = null;

        private const string throttleProfile6CharacteristicUuid = "0000ff06-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _throttleProfile6CharacteristicProxy = null;
        private Task? _throttleProfile6CharacteristicWatchTask = null;

        private readonly ScalextricArcState _scalextricArcState;
        private readonly Channel<CommandState> _commandStateChannel;
        private readonly ILogger<BluezMonitorService> _logger;


        public BluezMonitorService(ScalextricArcState scalextricArcState,
                                   Channel<CommandState> commandStateChannel,
                                   ILogger<BluezMonitorService> logger)
        {
            _scalextricArcState = scalextricArcState;
            _commandStateChannel = commandStateChannel;
            _logger = logger;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ = BluezDiscoveryAsync(cancellationToken);
            _ = CommandAsync(cancellationToken);
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

            await _scalextricArcState.ConnectionState.SetAsync(ConnectionStateType.Disabled);

            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                _logger.LogCritical("You need to be running Linux for this application.");
                return;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                _bluezObjectPathInterfaces = new();
                scalextricArcObjectPath = null;
                scalextricArcProxy = null;
                _throttleCharacteristicWatchTask = null;
                _slotCharacteristicWatchTask = null;

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
                        await _scalextricArcState.ConnectionState.SetAsync(ConnectionStateType.Disabled);
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
                    await _scalextricArcState.ConnectionState.SetAsync(ConnectionStateType.Enabled);

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
                        await _scalextricArcState.ConnectionState.SetAsync(ConnectionStateType.Disabled);
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
                            _logger.LogError(exception, exception.Message);
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
                            _logger.LogError(exception, exception.Message);
                        }
                    );

                    Console.WriteLine($"Creating bluez.DBus.IAdapter1 proxy: {bluezService} {bluezAdapterObjectPathKp.Key}");
                    var bluezAdapterProxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IAdapter1>(bluezService, bluezAdapterObjectPathKp.Key);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var scalextricArcObjectPathKps = _bluezObjectPathInterfaces.Where(x => x.Value.Any(i => i.BluezInterface == bluezDeviceInterface && !string.IsNullOrEmpty(i.DeviceName) && i.DeviceName.Trim() == "Scalextric ARC"));

                        if (!scalextricArcObjectPathKps.Any())
                        {
                            _commandCharacteristicProxy = null;

                            if (_slotCharacteristicWatchTask is not null)
                            {
                                _slotCharacteristicWatchTask.Dispose();
                                _slotCharacteristicWatchTask = null;
                            }

                            if (_slotCharacteristicProxy is not null)
                            {
                                if (await _slotCharacteristicProxy.GetNotifyingAsync())
                                {
                                    await _slotCharacteristicProxy.StopNotifyAsync();
                                }
                                _slotCharacteristicProxy = null;
                            }

                            if (_throttleCharacteristicWatchTask is not null)
                            {
                                _throttleCharacteristicWatchTask.Dispose();
                                _throttleCharacteristicWatchTask = null;
                            }

                            if (_throttleCharacteristicProxy is not null)
                            {
                                if (await _throttleCharacteristicProxy.GetNotifyingAsync())
                                {
                                    await _throttleCharacteristicProxy.StopNotifyAsync();
                                }
                                _throttleCharacteristicProxy = null;
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
                            await _scalextricArcState.ConnectionState.SetAsync(ConnectionStateType.Discovering);
                        }
                        else
                        {
                            //Console.WriteLine("Bluetooth device discovery not needed, device already found.");
                            if (await bluezAdapterProxy.GetDiscoveringAsync())
                            {
                                _logger.LogInformation("Stopping Bluetooth device discovery.");
                                await bluezAdapterProxy.StopDiscoveryAsync();
                                await _scalextricArcState.ConnectionState.SetAsync(ConnectionStateType.Enabled);
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

                        //_logger.LogInformation("BluezMonitorService is running...");

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
                            _logger.LogError(exception, exception.ErrorName);
                            break;

                        case "org.bluez.Error.Failed":
                            _logger.LogError(exception, exception.ErrorName);
                            break;

                        default:
                            _logger.LogError(exception, exception.Message);
                            break;
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.GetType().FullName);
                    _logger.LogError(exception, exception.Message);
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
                    await _scalextricArcState.ConnectionState.SetAsync(ConnectionStateType.Connected);

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

                        foreach (var item in _bluezObjectPathInterfaces.Where(x => x.Key.ToString().StartsWith(scalextricArcObjectPath.ToString()!) && x.Value.Any(i => i.BluezInterface == bluezGattCharacteristicInterface)).OrderBy(x => x.Key))
                        {
                            //Console.WriteLine($"GattCharacteristic: {item.Key} {string.Join(", ", item.Value.Select(x => x.BluezInterface))}");
                            var proxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IGattCharacteristic1>(bluezService, item.Key);
                            var properties = await proxy.GetAllAsync();
                            //Console.WriteLine($"UUID={properties.UUID}");
                            //Console.WriteLine($"Service={properties.Service}");
                            //Console.WriteLine($"Flags={string.Join(", ", properties.Flags!)}");

                            var gattCharacteristic = new GattCharacteristic();

                            if (!string.IsNullOrEmpty(properties.UUID))
                            {
                                gattCharacteristic.uuid = properties.UUID;

                                switch (properties.UUID)
                                {
                                    case manufacturerNameCharacteristicUuid:
                                        gattCharacteristic.Name = "Manufacturer name";
                                        break;

                                    case modelNumberCharacteristicUuid:
                                        gattCharacteristic.Name = "Model number";
                                        break;

                                    case hardwareRevisionCharacteristicUuid:
                                        gattCharacteristic.Name = "Hardware revision";
                                        break;

                                    case firmwareRevisionCharacteristicUuid:
                                        gattCharacteristic.Name = "Firmware revision";
                                        break;

                                    case softwareRevisionCharacteristicUuid:
                                        gattCharacteristic.Name = "Software revision";
                                        break;

                                    case dfuControlPointCharacteristicUuid:
                                        gattCharacteristic.Name = "DFU control point";
                                        break;

                                    case dfuPacketCharacteristicUuid:
                                        gattCharacteristic.Name = "DFU packet";
                                        break;

                                    case dfuRevisionCharacteristicUuid:
                                        gattCharacteristic.Name = "DFU revision";
                                        break;

                                    case commandCharacteristicUuid:
                                        gattCharacteristic.Name = "Command";
                                        break;

                                    case slotCharacteristicUuid:
                                        gattCharacteristic.Name = "Slot";
                                        break;

                                    case throttleCharacteristicUuid:
                                        gattCharacteristic.Name = "Throttle";
                                        break;

                                    case throttleProfile1CharacteristicUuid:
                                        gattCharacteristic.Name = "Throttle profile 1";
                                        break;

                                    case throttleProfile2CharacteristicUuid:
                                        gattCharacteristic.Name = "Throttle profile 2";
                                        break;

                                    case throttleProfile3CharacteristicUuid:
                                        gattCharacteristic.Name = "Throttle profile 3";
                                        break;

                                    case throttleProfile4CharacteristicUuid:
                                        gattCharacteristic.Name = "Throttle profile 4";
                                        break;

                                    case throttleProfile5CharacteristicUuid:
                                        gattCharacteristic.Name = "Throttle profile 5";
                                        break;

                                    case throttleProfile6CharacteristicUuid:
                                        gattCharacteristic.Name = "Throttle profile 6";
                                        break;

                                    default:
                                        break;
                                }
                            }

                            if (properties.Flags is not null)
                            {
                                foreach (var flag in properties.Flags)
                                {
                                    if (!string.IsNullOrEmpty(flag))
                                    {
                                        gattCharacteristic.Flags.Add(new GattCharacteristicFlag
                                        {
                                            Flag = flag
                                        });
                                    }                                    
                                }

                                if (properties.Flags!.Contains("read"))
                                {
                                    var value = await proxy.ReadValueAsync(new Dictionary<string, object>());
                                    gattCharacteristic.Length = value.Length;
                                    if (value.Length > 0)
                                    {
                                        if (!value.Any(x => x < 32))
                                        {
                                            gattCharacteristic.Value = System.Text.Encoding.UTF8.GetString(value);
                                        }

                                        if (properties.UUID == commandCharacteristicUuid)
                                        {
                                            await _scalextricArcState.CommandState.SetAsync
                                            (
                                                (CommandType)value[0],
                                                (byte)(value[1] & 0b111111),
                                                (value[1] & 0b10000000) > 0,
                                                value[7],
                                                value[13],
                                                (value[19] & 0b1) > 0,
                                                (byte)(value[2] & 0b111111),
                                                (value[2] & 0b10000000) > 0,
                                                value[8],
                                                value[14],
                                                (value[19] & 0b10) > 0,
                                                (byte)(value[3] & 0b111111),
                                                (value[3] & 0b10000000) > 0,
                                                value[9],
                                                value[15],
                                                (value[19] & 0b100) > 0,
                                                (byte)(value[4] & 0b111111),
                                                (value[4] & 0b10000000) > 0,
                                                value[10],
                                                value[16],
                                                (value[19] & 0b1000) > 0,
                                                (byte)(value[5] & 0b111111),
                                                (value[5] & 0b10000000) > 0,
                                                value[11],
                                                value[17],
                                                (value[19] & 0b10000) > 0,
                                                (byte)(value[6] & 0b111111),
                                                (value[6] & 0b10000000) > 0,
                                                value[12],
                                                value[18],
                                                (value[19] & 0b100000) > 0,
                                                false
                                            );
                                        }

                                        if (properties.UUID == throttleProfile1CharacteristicUuid)
                                        {
                                            foreach (var b in value)
                                            {
                                                _logger.LogInformation($"throttleProfile1CharacteristicUuid Value {b}");
                                            }
                                        }
                                    }
                                }
                            }

                            if (properties.UUID == commandCharacteristicUuid)
                            {
                                _commandCharacteristicProxy = proxy;
                            }

                            if (properties.UUID == slotCharacteristicUuid)
                            {
                                _slotCharacteristicProxy = proxy;
                                await _slotCharacteristicProxy.StartNotifyAsync();
                                _slotCharacteristicWatchTask = _slotCharacteristicProxy.WatchPropertiesAsync(slotCharacteristicWatchProperties);
                            }

                            if (properties.UUID == throttleCharacteristicUuid)
                            {
                                _throttleCharacteristicProxy = proxy;
                                await _throttleCharacteristicProxy.StartNotifyAsync();
                                _throttleCharacteristicWatchTask = _throttleCharacteristicProxy.WatchPropertiesAsync(throttleCharacteristicWatchProperties);
                            }

                            if (properties.UUID == throttleProfile1CharacteristicUuid)
                            {
                                _throttleProfile1CharacteristicProxy = proxy;
                                await _throttleProfile1CharacteristicProxy.StartNotifyAsync();
                                _throttleProfile1CharacteristicWatchTask = _throttleProfile1CharacteristicProxy.WatchPropertiesAsync(throttleProfile1CharacteristicWatchProperties);
                            }

                            _scalextricArcState.GattCharacteristics.Add(gattCharacteristic);
                        }

                        _logger.LogInformation("Connection to Scalextric ARC has been completed.");
                        await _scalextricArcState.ConnectionState.SetAsync(ConnectionStateType.Initialized);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.GetType().FullName);
                    _logger.LogError(exception, exception.Message);
                }
            }
        }


        private void throttleCharacteristicWatchProperties(Tmds.DBus.PropertyChanges propertyChanges)
        {
            foreach (var item in propertyChanges.Changed)
            {
                if (item.Key == "Value")
                {
                    var value = (byte[])item.Value;
                    //Console.WriteLine($"PS={value[0]}, 1={value[1]}, 2={value[2]}, 3={value[3]}, 4={value[4]}, 5={value[5]}, 6={value[6]}, AD={value[11] & 0b1}");
                    //Console.WriteLine($"brake1={(value[1] & 0b1000000) > 0}, LC1={(value[1] & 0b10000000) > 0}, LC1D={(value[11] & 0b100) > 0}");
                    //Console.WriteLine($"timestamp1={value[7]}, timestamp2={value[8]}, timestamp3={value[9]}, timestamp14={value[10]}, timestamp={(value[7] + value[8] * 2 ^ 8 + value[9] * 2 ^ 16 + value[10] * 2 ^ 24)}");

                    _scalextricArcState.ThrottleState!.Set
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
                        (uint)(value[7] + value[8] * 256 + value[9] * 65536 + value[10] * 16777216)
                    );
                }
            }
        }


        private void throttleProfile1CharacteristicWatchProperties(Tmds.DBus.PropertyChanges propertyChanges)
        {
            foreach (var item in propertyChanges.Changed)
            {
                _logger.LogInformation($"throttleProfile1CharacteristicWatchProperties: {item.Key} {item.Value}");
            }
        }


        public void slotCharacteristicWatchProperties(Tmds.DBus.PropertyChanges propertyChanges)
        {
            foreach (var item in propertyChanges.Changed)
            {
                if (item.Key == "Value")
                {
                    var value = (byte[])item.Value;
                    //Console.WriteLine($"PS={value[0]}, ID={value[1]}, {(uint)(value[2] + value[3] * 256 + value[4] * 65536 + value[5] * 16777216)}, {(uint)(value[6] + value[7] * 256 + value[8] * 65536 + value[9] * 16777216)}, {(uint)(value[10] + value[11] * 256 + value[12] * 65536 + value[13] * 16777216)}, {(uint)(value[14] + value[15] * 256 + value[16] * 65536 + value[17] * 16777216)}");

                    _scalextricArcState.SlotStates[value[1] - 1].Set
                    (
                        value[0],
                        (uint)(value[2] + value[3] * 256 + value[4] * 65536 + value[5] * 16777216),
                        (uint)(value[6] + value[7] * 256 + value[8] * 65536 + value[9] * 16777216),
                        (uint)(value[10] + value[11] * 256 + value[12] * 65536 + value[13] * 16777216),
                        (uint)(value[14] + value[15] * 256 + value[16] * 65536 + value[17] * 16777216)
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


        private async Task CommandAsync(CancellationToken cancellationToken)
        {
            await foreach (var commandState in _commandStateChannel.Reader.ReadAllAsync(cancellationToken))
            {
                if (_commandCharacteristicProxy is not null)
                {
                    var value = new byte[20];
                    value[0] = (byte)commandState.Command;
                    value[1] = (byte)(commandState.PowerMultiplier1 + (commandState.Ghost1 ? 128 : 0));
                    value[2] = (byte)(commandState.PowerMultiplier2 + (commandState.Ghost2 ? 128 : 0));
                    value[3] = (byte)(commandState.PowerMultiplier3 + (commandState.Ghost3 ? 128 : 0));
                    value[4] = (byte)(commandState.PowerMultiplier4 + (commandState.Ghost4 ? 128 : 0));
                    value[5] = (byte)(commandState.PowerMultiplier5 + (commandState.Ghost5 ? 128 : 0));
                    value[6] = (byte)(commandState.PowerMultiplier6 + (commandState.Ghost6 ? 128 : 0));
                    value[7] = commandState.Rumble1;
                    value[8] = commandState.Rumble2;
                    value[9] = commandState.Rumble3;
                    value[10] = commandState.Rumble4;
                    value[11] = commandState.Rumble5;
                    value[12] = commandState.Rumble6;
                    value[13] = commandState.Brake1;
                    value[14] = commandState.Brake2;
                    value[15] = commandState.Brake3;
                    value[16] = commandState.Brake4;
                    value[17] = commandState.Brake5;
                    value[18] = commandState.Brake6;
                    value[19] = (byte)((commandState.Kers1 ? 1 : 0) + (commandState.Kers2 ? 2 : 0) + (commandState.Kers3 ? 4 : 0) + (commandState.Kers4 ? 8 : 0) + (commandState.Kers5 ? 16 : 0) + (commandState.Kers6 ? 32 : 0));
                    await _commandCharacteristicProxy.WriteValueAsync(value, new Dictionary<string, object>());
                }
            }
        }
    }
}
