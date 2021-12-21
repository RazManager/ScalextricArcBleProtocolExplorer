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
        private bluez.DBus.IAdapter1? _bluezAdapterProxy = null;
        private Tmds.DBus.ObjectPath? _scalextricArcObjectPath = null;
        private bluez.DBus.IDevice1? _scalextricArcProxy = null;

        private const string manufacturerNameCharacteristicUuid = "00002a29-0000-1000-8000-00805f9b34fb";
        private const string modelNumberCharacteristicUuid = "00002a24-0000-1000-8000-00805f9b34fb";
        private const string hardwareRevisionCharacteristicUuid = "00002a27-0000-1000-8000-00805f9b34fb";
        private const string firmwareRevisionCharacteristicUuid = "00002a26-0000-1000-8000-00805f9b34fb";
        private const string softwareRevisionCharacteristicUuid = "00002a28-0000-1000-8000-00805f9b34fb";
        private const string dfuControlPointCharacteristicUuid = "00001531-1212-efde-1523-785feabcd123";
        private const string dfuPacketCharacteristicUuid = "00001532-1212-efde-1523-785feabcd123";
        private const string dfuRevisionCharacteristicUuid = "00001534-1212-efde-1523-785feabcd123";
        private const string serviceChangedCharacteristicUuid = "00002a05-0000-1000-8000-00805f9b34fb";

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

        private const string throttleProfile2CharacteristicUuid = "0000ff02-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _throttleProfile2CharacteristicProxy = null;

        private const string throttleProfile3CharacteristicUuid = "0000ff03-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _throttleProfile3CharacteristicProxy = null;

        private const string throttleProfile4CharacteristicUuid = "0000ff04-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _throttleProfile4CharacteristicProxy = null;

        private const string throttleProfile5CharacteristicUuid = "0000ff05-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _throttleProfile5CharacteristicProxy = null;

        private const string throttleProfile6CharacteristicUuid = "0000ff06-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _throttleProfile6CharacteristicProxy = null;

        private const string trackCharacteristicUuid = "00003b0c-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _trackCharacteristicProxy = null;
        private Task? _trackCharacteristicWatchTask = null;

        private const string carIdCharacteristicUuid = "00003b0d-0000-1000-8000-00805f9b34fb";
        private bluez.DBus.IGattCharacteristic1? _carIdCharacteristicProxy = null;

        private readonly ScalextricArcState _scalextricArcState;
        private readonly Channel<CarIdState> _carIdStateChannel;
        private readonly Channel<CommandState> _commandStateChannel;
        private readonly Channel<ConnectionDto> _connectionChannel;
        private readonly Channel<ThrottleProfileState> _throttleProfileStateChannel;
        private readonly ILogger<BluezMonitorService> _logger;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task? _bluezDiscoveryTask;
        private Task? _carIdTask;
        private Task? _commandTask;
        private Task? _throttleProfileTask;
        private Task? _connectionTask;


        private bool _discoveryStarted = false;


        public BluezMonitorService(ScalextricArcState scalextricArcState,
                                   Channel<CarIdState> carIdStateChannel,
                                   Channel<CommandState> commandStateChannel,
                                   Channel<ConnectionDto> connectionChannel,
                                   Channel<ThrottleProfileState> throttleProfileStateChannel,
                                   ILogger<BluezMonitorService> logger)
        {
            _scalextricArcState = scalextricArcState;
            _carIdStateChannel = carIdStateChannel;
            _commandStateChannel = commandStateChannel;
            _connectionChannel = connectionChannel;
            _throttleProfileStateChannel = throttleProfileStateChannel;
            _logger = logger;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            var resetResult = _cancellationTokenSource.TryReset();
            Console.WriteLine($"resetResult={resetResult}");

            _bluezDiscoveryTask = BluezDiscoveryAsync(_cancellationTokenSource.Token);
            _carIdTask = CarIdAsync(_cancellationTokenSource.Token);
            _commandTask = CommandAsync(_cancellationTokenSource.Token);
            _throttleProfileTask = ThrottleProfileAsync(_cancellationTokenSource.Token);

            if (_connectionTask is null)
            {
                _connectionTask = ConnectionAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();

            if (_bluezDiscoveryTask is not null)
            {
                try
                {
                    _bluezDiscoveryTask.Wait();
                }
                catch (Exception)
                {
                }
            }
            if (_carIdTask is not null)
            {
                try
                {
                    _carIdTask.Wait();
                }
                catch (Exception)
                {
                }
            }
            if (_commandTask is not null)
            {
                try
                {
                    _commandTask.Wait();
                }
                catch (Exception)
                {
                }
            }
            if (_throttleProfileTask is not null)
            {
                try
                {
                    _throttleProfileTask.Wait();
                }
                catch (Exception)
                {
                }
            }

            return Task.CompletedTask;
        }


        private async Task BluezDiscoveryAsync(CancellationToken cancellationToken)
        {
            Task? watchInterfacesAddedTask = null;
            Task? watchInterfacesRemovedTask = null;

            await _scalextricArcState.ConnectionState.SetBluetoothStateAsync(BluetoothConnectionStateType.Disabled);

            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                _logger.LogCritical("You need to be running Linux for this application.");
                return;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                _bluezObjectPathInterfaces = new();
                _scalextricArcObjectPath = null;
                _scalextricArcProxy = null;
                _slotCharacteristicWatchTask = null;
                _throttleCharacteristicWatchTask = null;
                _discoveryStarted = false;

                try
                {
                    // Find all D-Bus objects and their interfaces
                    var objectManager = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IObjectManager>(bluezService, Tmds.DBus.ObjectPath.Root);
                    var dBusObjects = await objectManager.GetManagedObjectsAsync();
                    foreach (var dBusObject in dBusObjects)
                    {
                        InterfaceAdded((dBusObject.Key, dBusObject.Value));
                    }

                    var bluezAdapterObjectPathKp = _bluezObjectPathInterfaces.SingleOrDefault(x => x.Value.Any(i => i.BluezInterface == bluezAdapterInterface));
                    if (string.IsNullOrEmpty(bluezAdapterObjectPathKp.Key.ToString()))
                    {
                        _logger.LogError($"{bluezAdapterInterface} does not exist. Please install BlueZ for the needed Bluetooth Low Energy functionality, and then re-start this application.");
                        await _scalextricArcState.ConnectionState.SetBluetoothStateAsync(BluetoothConnectionStateType.Disabled);
                        return;
                    }

                    _bluezAdapterProxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IAdapter1>(bluezService, bluezAdapterObjectPathKp.Key);

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

                    _logger.LogInformation("BlueZ initialization done. Trying to find a Scalextric ARC device...");
                    await _scalextricArcState.ConnectionState.SetBluetoothStateAsync(BluetoothConnectionStateType.Enabled);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var scalextricArcObjectPathKps = _bluezObjectPathInterfaces.Where(x => x.Value.Any(i => i.BluezInterface == bluezDeviceInterface && !string.IsNullOrEmpty(i.DeviceName) && i.DeviceName.Trim() == "Scalextric ARC"));

                        if (!scalextricArcObjectPathKps.Any())
                        {
                            await ResetAsync();

                            if (await _bluezAdapterProxy.GetDiscoveringAsync())
                            {
                                _logger.LogInformation("Searching...");
                            }
                            else
                            {
                                var discoveryProperties = new Dictionary<string, object>
                                {
                                    {
                                        "UUIDs",
                                        new string[] { "00003b08-0000-1000-8000-00805f9b34fb" }
                                    }
                                };
                                await _bluezAdapterProxy.SetDiscoveryFilterAsync(discoveryProperties);
                                _logger.LogInformation("Starting Bluetooth device discovery.");
                                await _bluezAdapterProxy.StartDiscoveryAsync();
                                _discoveryStarted = true;

                                //Name=Scalextric ARC
                                //Alias=Scalextric ARC
                                //Appearance=833
                                //Values for property=UUIDs:
                                //00003b08-0000-1000-8000-00805f9b34fb
                                //0000180a-0000-1000-8000-00805f9b34fb
                            }
                            await _scalextricArcState.ConnectionState.SetBluetoothStateAsync(BluetoothConnectionStateType.Discovering);
                        }
                        else
                        {
                            // Bluetooth device discovery not needed, device already found.
                            if (_scalextricArcProxy is not null && await _bluezAdapterProxy.GetDiscoveringAsync() && _discoveryStarted)
                            {
                                _logger.LogInformation("Stopping Bluetooth device discovery.");
                                await _bluezAdapterProxy.StopDiscoveryAsync();
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
                    _logger.LogError(exception, $"{exception.ErrorName}, {exception.ErrorMessage}");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }

            await ResetAsync();
        }


        private void InterfaceAdded((Tmds.DBus.ObjectPath objectPath, IDictionary<string, IDictionary<string, object>> interfaces) args)
        {
            Console.WriteLine($"{args.objectPath} added.");
            // Console.WriteLine($"{args.objectPath} added with the following interfaces...");
            // foreach (var iface in args.interfaces)
            // {
            //     Console.WriteLine(iface.Key);
            // }

            if (args.interfaces.Keys.Any(x => x.StartsWith(bluezService)))
            {
                var bluezInterfaceMetadata = new List<BluezInterfaceMetadata>();
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
                    bluezInterfaceMetadata.Add(new BluezInterfaceMetadata
                    {
                        BluezInterface = item.Key,
                        UUID = uuid,
                        DeviceName = item.Value.SingleOrDefault(x => item.Key == bluezDeviceInterface && x.Key == "Name").Value?.ToString()?.Trim()
                    });
                }

                _bluezObjectPathInterfaces.TryAdd(args.objectPath, bluezInterfaceMetadata);
            }
        }


        private void InterfaceRemoved((Tmds.DBus.ObjectPath objectPath, string[] interfaces) args)
        {
            Console.WriteLine($"{args.objectPath} removed.");
            _bluezObjectPathInterfaces.TryRemove(args.objectPath, out IEnumerable<BluezInterfaceMetadata>? bluezInterfaceMetadata);
        }


        private async Task ScalextricArcChangedAsync(Tmds.DBus.ObjectPath objectPath)
        {
            if (_scalextricArcObjectPath is null)
            {
                try
                {
                    _scalextricArcProxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IDevice1>(bluezService, objectPath);

                    if (await _scalextricArcProxy.GetConnectedAsync())
                    {
                        _logger.LogInformation("Scalextric ARC already connected.");
                        _scalextricArcObjectPath = objectPath;
                    }
                    else
                    {
                        _logger.LogInformation($"Connecting to Scalextric ARC... {await _scalextricArcProxy.GetAddressAsync()} #{await _scalextricArcProxy.GetNameAsync()}#");
                        bool success = false;
                        for (int i = 1; i <= 5; i++)
                        {
                            try
                            {
                                await _scalextricArcProxy.ConnectAsync();
                                success = true;
                                break;
                            }
                            catch (Tmds.DBus.DBusException exception)
                            {
                                _logger.LogInformation($"Connection attempt {i}(5) failed: {exception.ErrorName}, {exception.ErrorMessage}");
                                await Task.Delay(TimeSpan.FromSeconds(5));
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                        if (success)
                        {
                            _scalextricArcObjectPath = objectPath;
                            await _scalextricArcState.ConnectionState.SetBluetoothStateAsync(BluetoothConnectionStateType.Connected);
                            _logger.LogInformation("Connected to Scalextric ARC.");
                        }
                        else
                        {
                            await _scalextricArcState.ConnectionState.SetBluetoothStateAsync(BluetoothConnectionStateType.Enabled);
                            _logger.LogInformation("Could not connect to Scalextric ARC.");
                            await ResetAsync();
                        }
                    }

                    if (_scalextricArcObjectPath != null)
                    {
                        _logger.LogInformation("Initiating Scalextric ARC services.");

                        var deviceProperties = await _scalextricArcProxy.GetAllAsync();
                        _logger.LogInformation($"Address={deviceProperties.Address}");
                        _logger.LogInformation($"AddressType={deviceProperties.AddressType}");
                        _logger.LogInformation($"Name={deviceProperties.Name}");
                        _logger.LogInformation($"Alias={deviceProperties.Alias}");
                        _logger.LogInformation($"Class={deviceProperties.Class}");
                        _logger.LogInformation($"Appearance={deviceProperties.Appearance}");
                        //_logger.LogInformation($"Paired={deviceProperties.Paired}");
                        //_logger.LogInformation($"Trusted={deviceProperties.Trusted}");
                        //_logger.LogInformation($"Blocked={deviceProperties.Blocked}");
                        //_logger.LogInformation($"LegacyPairing={deviceProperties.LegacyPairing}");
                        _logger.LogInformation($"RSSI={deviceProperties.RSSI}");
                        //_logger.LogInformation($"Connected={deviceProperties.Connected}");
                        _logger.LogInformation($"UUIDs={string.Join(", ", deviceProperties.UUIDs!)}");
                        _logger.LogInformation($"Adapter={deviceProperties.Adapter}");
                        _logger.LogInformation($"TxPower={deviceProperties.TxPower}");
                        //_logger.LogInformation($"ServicesResolved={deviceProperties.ServicesResolved}");
                        //_logger.LogInformation($"WakeAllowed={deviceProperties.WakeAllowed}");
                        //public IDictionary<ushort, object>? ManufacturerData
                        //public IDictionary<string, object>? ServiceData

                        if (!await _scalextricArcProxy.GetServicesResolvedAsync())
                        {
                            _logger.LogInformation("Waiting for Scalextric ARC services to be resolved...");
                            for (int i = 1; i <= 5; i++)
                            {
                                if (await _scalextricArcProxy.GetServicesResolvedAsync())
                                {
                                    break;
                                }

                                await Task.Delay(TimeSpan.FromSeconds(5));
                            }

                            if (!await _scalextricArcProxy.GetServicesResolvedAsync())
                            {
                                await _scalextricArcState.ConnectionState.SetBluetoothStateAsync(BluetoothConnectionStateType.Enabled);
                                throw new Exception("Scalextric ARC services could not be resolved");
                                await ResetAsync();
                            }
                        }

                        //Console.WriteLine("Bluez objects and interfaces");
                        //foreach (var item in _bluezObjectPathInterfaces)
                        //{
                        //    Console.WriteLine($"{item.Key} {string.Join(", ", item.Value.Select(x => x.BluezInterface))}");
                        //}

                        _scalextricArcState.GattCharacteristics = new();

                        foreach (var item in _bluezObjectPathInterfaces.Where(x => x.Key.ToString().StartsWith(_scalextricArcObjectPath.ToString()!) && x.Value.Any(i => i.BluezInterface == bluezGattCharacteristicInterface)).OrderBy(x => x.Key))
                        {
                            var proxy = Tmds.DBus.Connection.System.CreateProxy<bluez.DBus.IGattCharacteristic1>(bluezService, item.Key);
                            var properties = await proxy.GetAllAsync();

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

                                    case serviceChangedCharacteristicUuid:
                                        gattCharacteristic.Name = "Service changed";
                                        break;

                                    case carIdCharacteristicUuid:
                                        gattCharacteristic.Name = "Car ID";
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

                                    case trackCharacteristicUuid:
                                        gattCharacteristic.Name = "Track";
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

                                        if (properties.UUID == carIdCharacteristicUuid)
                                        {
                                            await _scalextricArcState.CarIdState.SetAsync
                                            (
                                                value[0],
                                                false
                                            );
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
                                    }
                                }
                            }

                            if (properties.UUID == carIdCharacteristicUuid)
                            {
                                _carIdCharacteristicProxy = proxy;
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
                            }

                            if (properties.UUID == throttleProfile2CharacteristicUuid)
                            {
                                _throttleProfile2CharacteristicProxy = proxy;
                            }

                            if (properties.UUID == throttleProfile3CharacteristicUuid)
                            {
                                _throttleProfile3CharacteristicProxy = proxy;
                            }

                            if (properties.UUID == throttleProfile4CharacteristicUuid)
                            {
                                _throttleProfile4CharacteristicProxy = proxy;
                            }

                            if (properties.UUID == throttleProfile5CharacteristicUuid)
                            {
                                _throttleProfile5CharacteristicProxy = proxy;
                            }

                            if (properties.UUID == throttleProfile6CharacteristicUuid)
                            {
                                _throttleProfile6CharacteristicProxy = proxy;
                            }

                            if (properties.UUID == trackCharacteristicUuid)
                            {
                                _trackCharacteristicProxy = proxy;
                                await _trackCharacteristicProxy.StartNotifyAsync();
                                _trackCharacteristicWatchTask = _trackCharacteristicProxy.WatchPropertiesAsync(trackCharacteristicWatchProperties);
                            }

                            _scalextricArcState.GattCharacteristics.Add(gattCharacteristic);
                        }

                        await _scalextricArcState.ConnectionState.SetBluetoothStateAsync(BluetoothConnectionStateType.Initialized);
                        _logger.LogInformation("Scalextric ARC services have been initialized.");
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                }
            }
        }


        private async Task ResetAsync()
        {
            _carIdCharacteristicProxy = null;

            _commandCharacteristicProxy = null;

            if (_slotCharacteristicWatchTask is not null)
            {
                _slotCharacteristicWatchTask.Dispose();
                _slotCharacteristicWatchTask = null;
            }

            _slotCharacteristicProxy = null;

            if (_throttleCharacteristicWatchTask is not null)
            {
                _throttleCharacteristicWatchTask.Dispose();
                _throttleCharacteristicWatchTask = null;
            }

            _throttleCharacteristicProxy = null;

            _throttleProfile1CharacteristicProxy = null;
            _throttleProfile2CharacteristicProxy = null;
            _throttleProfile3CharacteristicProxy = null;
            _throttleProfile4CharacteristicProxy = null;
            _throttleProfile5CharacteristicProxy = null;
            _throttleProfile6CharacteristicProxy = null;

            if (_trackCharacteristicWatchTask is not null)
            {
                _trackCharacteristicWatchTask.Dispose();
                _trackCharacteristicWatchTask = null;
            }

            _trackCharacteristicProxy = null;

            if (_scalextricArcProxy is not null)
            {
                if (await _scalextricArcProxy.GetConnectedAsync())
                {
                    try
                    {
                        _logger.LogInformation("Disconnecting Scalextric ARC...");
                        await _scalextricArcProxy.DisconnectAsync();                        
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, exception.Message);
                    }
                    _logger.LogInformation("Scalextric ARC disconnected.");
                }

                try
                {
                    _logger.LogInformation("Cancelling paring for Scalextric ARC...");
                    await _scalextricArcProxy.CancelPairingAsync();
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                }
                _logger.LogInformation("Scalextric ARC pairing cancelled.");


                _scalextricArcProxy = null;
            }

            Console.WriteLine($"_bluezAdapterProxy is not null: {_bluezAdapterProxy is not null}   _scalextricArcObjectPath.HasValue: {_scalextricArcObjectPath.HasValue}");
            if (_bluezAdapterProxy is not null && _scalextricArcObjectPath.HasValue)
            {
                try
                {
                    _logger.LogInformation("Removing for Scalextric ARC...");
                    await _bluezAdapterProxy.RemoveDeviceAsync(_scalextricArcObjectPath.Value);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                }
                _logger.LogInformation("Scalextric ARC removed.");
            }

            _scalextricArcObjectPath = null;
        }


        private void slotCharacteristicWatchProperties(Tmds.DBus.PropertyChanges propertyChanges)
        {
            foreach (var item in propertyChanges.Changed)
            {
                if (item.Key == "Value")
                {
                    var value = (byte[])item.Value;
                    _scalextricArcState.SlotStates[value[1] - 1].SetAsync
                    (
                        value[0],
                        (uint)(value[2] + value[3] * 256 + value[4] * 65536 + value[5] * 16777216),
                        (uint)(value[6] + value[7] * 256 + value[8] * 65536 + value[9] * 16777216),
                        (uint)(value[10] + value[11] * 256 + value[12] * 65536 + value[13] * 16777216),
                        (uint)(value[14] + value[15] * 256 + value[16] * 65536 + value[17] * 16777216)
                    ).Wait();
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
                    _scalextricArcState.ThrottleState!.SetAsync
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
                    ).Wait();
                }
            }
        }


        private void trackCharacteristicWatchProperties(Tmds.DBus.PropertyChanges propertyChanges)
        {
            foreach (var item in propertyChanges.Changed)
            {
                if (item.Key == "Value")
                {
                    var value = (byte[])item.Value;
                    _scalextricArcState.TrackState!.Set
                    (
                        value[0],
                        value[1],
                        value[2],
                        (uint)(value[3] + value[4] * 256 + value[5] * 65536 + value[6] * 16777216)
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
                                Console.WriteLine($"{prop.Key}=#{prop.Value}#");
                                break;
                        }
                    }
                }
            }
        }


        private async Task CarIdAsync(CancellationToken cancellationToken)
        {
            await foreach (var carIdState in _carIdStateChannel.Reader.ReadAllAsync(cancellationToken))
            {
                if (_carIdCharacteristicProxy is not null)
                {
                    var value = new byte[1];
                    value[0] = carIdState.CarId;
                    await _carIdCharacteristicProxy.WriteValueAsync(value, new Dictionary<string, object>());
                }
            }
        }


        private async Task CommandAsync(CancellationToken cancellationToken)
        {
            await foreach (var commandState in _commandStateChannel.Reader.ReadAllAsync(cancellationToken))
            {
                if (_commandCharacteristicProxy is not null)
                {
                    Console.WriteLine($"Command: {(byte)commandState.Command} {commandState.Command}");
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


        private async Task ConnectionAsync(CancellationToken cancellationToken)
        {
            await foreach (var connectionDto in _connectionChannel.Reader.ReadAllAsync(cancellationToken))
            {
                Console.WriteLine($"ConnectionAsync: {connectionDto.Connect}");
                if (connectionDto.Connect)
                {
                    await StartAsync(cancellationToken);
                }
                else
                {
                    await StopAsync(cancellationToken);
                }
            }
        }


        private async Task ThrottleProfileAsync(CancellationToken cancellationToken)
        {
            await foreach (var throttleProfileState in _throttleProfileStateChannel.Reader.ReadAllAsync(cancellationToken))
            {
                switch (throttleProfileState.CarId)
                {
                    case 1:
                        if (_throttleProfile1CharacteristicProxy is not null)
                        {
                            await ThrottleProfileWriteAsync(_throttleProfile1CharacteristicProxy, throttleProfileState.Values);
                        }
                        break;

                    case 2:
                        if (_throttleProfile2CharacteristicProxy is not null)
                        {
                            await ThrottleProfileWriteAsync(_throttleProfile2CharacteristicProxy, throttleProfileState.Values);
                        }
                        break;

                    case 3:
                        if (_throttleProfile3CharacteristicProxy is not null)
                        {
                            await ThrottleProfileWriteAsync(_throttleProfile3CharacteristicProxy, throttleProfileState.Values);
                        }
                        break;

                    case 4:
                        if (_throttleProfile4CharacteristicProxy is not null)
                        {
                            await ThrottleProfileWriteAsync(_throttleProfile4CharacteristicProxy, throttleProfileState.Values);
                        }
                        break;

                    case 5:
                        if (_throttleProfile5CharacteristicProxy is not null)
                        {
                            await ThrottleProfileWriteAsync(_throttleProfile5CharacteristicProxy, throttleProfileState.Values);
                        }
                        break;

                    case 6:
                        if (_throttleProfile6CharacteristicProxy is not null)
                        {
                            await ThrottleProfileWriteAsync(_throttleProfile6CharacteristicProxy, throttleProfileState.Values);
                        }
                        break;

                    default:
                        break;
                }
            }
        }


        private async Task ThrottleProfileWriteAsync(bluez.DBus.IGattCharacteristic1 proxy, IEnumerable<ThrottleProfileValueDto> values)
        {
            byte[] buffer = new byte[20];
            foreach (var item in values.Select((x, i) => new { Dto = x, Index = i}))
            {
                buffer[item.Index % 16 + 1] = item.Dto.Value;
                if (item.Index % 16 == 15)
                {
                    buffer[0] = (byte)(item.Index / 16);
                    //Console.WriteLine("Writing throttle profile buffer:");
                    //for (int i = 0; i < buffer.Length; i++)
                    //{
                    //    Console.WriteLine($"{i} {buffer[i]}");
                    //}
                    await proxy.WriteValueAsync(buffer, new Dictionary<string, object>());
                }
            }
        }
    }
}
