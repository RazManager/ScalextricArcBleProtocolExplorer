using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Linq;


namespace ScalextricArcBleProtocolExplorer.Services
{
    public class ScalextricArcState
    {
        public ConnectionState ConnectionState { get; set; }
        public CommandState CommandState { get; set; }
        public DeviceInformation DeviceInformation { get; set; } = new();
        public SlotState[] SlotStates { get; init; } = new SlotState[6];
        public ThrottleState ThrottleState { get; set; }

        public ScalextricArcState(IHubContext<Hubs.ConnectionHub, Hubs.IConnectionHub> connectionHubContext,
                                  IHubContext<Hubs.CommandHub, Hubs.ICommandHub> commandHubContext,
                                  Channel<CommandState> commandStateChannel,
                                  IHubContext<Hubs.SlotHub, Hubs.ISlotHub> slotHubContext,
                                  IHubContext<Hubs.ThrottleHub, Hubs.IThrottleHub> throttleHubContext)
        {
            ConnectionState = new ConnectionState(connectionHubContext);

            CommandState = new CommandState(commandHubContext, commandStateChannel);

            for (byte i = 0; i < SlotStates.Length; i++)
            {
                SlotStates[i] = new SlotState(slotHubContext) { CarId = (byte)(i + 1) };
            }

            ThrottleState = new ThrottleState(throttleHubContext);
        }
    }


    public class ConnectionState
    {
        private readonly IHubContext<Hubs.ConnectionHub, Hubs.IConnectionHub> _hubContext;

        public ConnectionState(IHubContext<Hubs.ConnectionHub, Hubs.IConnectionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public enum ConnectionStateType
        {
            Disabled,
            Enabled,
            Discovering,
            Connected,
            Initialized
        }

        [Required]
        public ConnectionStateType State { get; set; }

        public async Task SetAsync(ConnectionStateType state)
        {
            State = state;
            await _hubContext.Clients.All.ChangedState(this);
        }
    }


    public enum CommandType
    {
        NoPowerTimerStopped = 0,
        NoPowerTimerTicking = 1,
        PowerOnRaceTrigger = 2,
        PowerOnRacing = 3,
        PowerOnTimerHalt = 4,
        NoPowerRebootPic18 = 5
    }


    public class CommandDto
    {
        [Required]
        public CommandType Command { get; set; }

        [Required]
        public byte PowerMultiplier1 { get; set; }

        [Required]
        public bool Ghost1 { get; set; }

        [Required]
        public byte Rumble1 { get; set; }

        [Required]
        public byte Brake1 { get; set; }

        [Required]
        public bool Kers1 { get; set; }

        [Required]
        public byte PowerMultiplier2 { get; set; }

        [Required]
        public bool Ghost2 { get; set; }

        [Required]
        public byte Rumble2 { get; set; }

        [Required]
        public byte Brake2 { get; set; }

        [Required]
        public bool Kers2 { get; set; }

        [Required]
        public byte PowerMultiplier3 { get; set; }

        [Required]
        public bool Ghost3 { get; set; }

        [Required]
        public byte Rumble3 { get; set; }

        [Required]
        public byte Brake3 { get; set; }

        [Required]
        public bool Kers3 { get; set; }

        [Required]
        public byte PowerMultiplier4 { get; set; }

        [Required]
        public bool Ghost4 { get; set; }

        [Required]
        public byte Rumble4 { get; set; }

        [Required]
        public byte Brake4 { get; set; }

        [Required]
        public bool Kers4 { get; set; }

        [Required]
        public byte PowerMultiplier5 { get; set; }

        [Required]
        public bool Ghost5 { get; set; }

        [Required]
        public byte Rumble5 { get; set; }

        [Required]
        public byte Brake5 { get; set; }

        [Required]
        public bool Kers5 { get; set; }

        [Required]
        public byte PowerMultiplier6 { get; set; }

        [Required]
        public bool Ghost6 { get; set; }

        [Required]
        public byte Rumble6 { get; set; }

        [Required]
        public byte Brake6 { get; set; }

        [Required]
        public bool Kers6 { get; set; }
    }


    public class CommandState : CommandDto
    {
        private readonly IHubContext<Hubs.CommandHub, Hubs.ICommandHub> _hubContext;
        private readonly Channel<CommandState> _commandStateChannel;

        public CommandState(IHubContext<Hubs.CommandHub, Hubs.ICommandHub> hubContext,
                            Channel<CommandState> commandStateChannel)
        {
            _hubContext = hubContext;
            _commandStateChannel = commandStateChannel;
        }

        public async Task SetAsync
        (
            CommandType command,
            byte powerMultiplier1,
            bool ghost1,
            byte rumble1,
            byte brake1,
            bool kers1,
            byte powerMultiplier2,
            bool ghost2,
            byte rumble2,
            byte brake2,
            bool kers2,
            byte powerMultiplier3,
            bool ghost3,
            byte rumble3,
            byte brake3,
            bool kers3,
            byte powerMultiplier4,
            bool ghost4,
            byte rumble4,
            byte brake4,
            bool kers4,
            byte powerMultiplier5,
            bool ghost5,
            byte rumble5,
            byte brake5,
            bool kers5,
            byte powerMultiplier6,
            bool ghost6,
            byte rumble6,
            byte brake6,
            bool kers6,
            bool write
        )
        {
            Command = command;
            PowerMultiplier1 = powerMultiplier1;
            Ghost1 = ghost1;
            Rumble1 = rumble1;
            Brake1 = brake1;
            Kers1 = kers1;
            PowerMultiplier2 = powerMultiplier2;
            Ghost2 = ghost2;
            Rumble2 = rumble2;
            Brake2 = brake2;
            Kers2 = kers2;
            PowerMultiplier3 = powerMultiplier3;
            Ghost3 = ghost3;
            Rumble3 = rumble3;
            Brake3 = brake3;
            Kers3 = kers3;
            PowerMultiplier4 = powerMultiplier4;
            Ghost4 = ghost4;
            Rumble4 = rumble4;
            Brake4 = brake4;
            Kers4 = kers4;
            PowerMultiplier5 = powerMultiplier5;
            Ghost5 = ghost5;
            Rumble5 = rumble5;
            Brake5 = brake5;
            Kers5 = kers5;
            PowerMultiplier6 = powerMultiplier6;
            Ghost6 = ghost6;
            Rumble6 = rumble6;
            Brake6 = brake6;
            Kers6 = kers6;

            if (write)
            {
                await _commandStateChannel.Writer.WriteAsync(this);
            }

            await _hubContext.Clients.All.ChangedState(this);
        }
    }


    public class GattCharacteristic
    {
        [Required]
        public Guid uuid { get; set; }

        [Required]
        public List<GattCharacteristicFlag> Flags { get; init; } = new();
    }


    public class GattCharacteristicFlag
    {
        [Required]
        public string Flag { get; set; } = null!;
    }


    public class DeviceInformation
    {
        public string? ManufacturerName { get; set; }
        public string? ModelNumber { get; set; }
        public string? HardwareRevision { get; set; }
        public string? FirmwareRevision { get; set; }
        public string? SoftwareRevision { get; set; }
        public List<GattCharacteristic> GattCharacteristics { get; init; } = new();

        public void Set
        (
            string? manufacturerName,
            string? modelNumber,
            string? hardwareRevision,
            string? firmwareRevision,
            string? softwareRevision
        )
        {
            ManufacturerName = manufacturerName;
            ModelNumber = modelNumber;
            HardwareRevision = hardwareRevision;
            FirmwareRevision = firmwareRevision;
            SoftwareRevision = softwareRevision;
        }

        public void AddGattCharacteristicFlag(Guid uuid, string flag)
        {
            var gattCharacteristic = GattCharacteristics.SingleOrDefault(x => x.uuid == uuid);
            if (gattCharacteristic == null)
            {
                gattCharacteristic = new GattCharacteristic
                {
                    uuid = uuid
                };
                GattCharacteristics.Add(gattCharacteristic);
            }
            gattCharacteristic.Flags.Add(new GattCharacteristicFlag
            {
                Flag = flag
            });
        }
    }


    public class SlotState
    {
        private readonly IHubContext<Hubs.SlotHub, Hubs.ISlotHub> _hubContext;

        public SlotState(IHubContext<Hubs.SlotHub, Hubs.ISlotHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public byte? PacketSequence { get; set; } = null;
        [Required]
        public byte CarId { get; init; }
        public uint? TimestampStartFinish1 { get; set; } = null;
        private uint? TimestampStartFinish1Previous { get; set; } = null;
        public uint? TimestampStartFinish2 { get; set; } = null;
        private uint? TimestampStartFinish2Previous { get; set; } = null;
        public uint? TimestampPitlane1 { get; set; } = null;
        public uint? TimestampPitlane2 { get; set; } = null;
        private DateTimeOffset? TimestampRefreshRateLast { get; set; } = null;
        private DateTimeOffset? TimestampRefreshRatePrevious { get; set; } = null;

        public uint? TimestampStartFinishPitlaneInterval1
        {
            get
            {
                if (TimestampStartFinish1.HasValue && TimestampPitlane1.HasValue && TimestampStartFinish1.Value < TimestampPitlane1.Value)
                {
                    return TimestampPitlane1.Value - TimestampStartFinish1.Value;
                }
                return null;
            }
        }

        public uint? TimestampStartFinishPitlaneInterval2
        {
            get
            {
                if (TimestampStartFinish2.HasValue && TimestampPitlane2.HasValue && TimestampStartFinish2.Value < TimestampPitlane2.Value)
                {
                    return TimestampPitlane2.Value - TimestampStartFinish2.Value;
                }
                return null;
            }
        }

        public uint? Laptime
        {
            get
            {
                uint? last = null;
                if (TimestampStartFinish1.HasValue && TimestampStartFinish2.HasValue)
                {
                    last = Math.Max(TimestampStartFinish1.Value, TimestampStartFinish2.Value);
                }
                else if (TimestampStartFinish1.HasValue)
                {
                    last = TimestampStartFinish1.Value;
                }
                else if (TimestampStartFinish2.HasValue)
                {
                    last = TimestampStartFinish2.Value;
                }

                uint? previous = null;
                if (TimestampStartFinish1Previous.HasValue && TimestampStartFinish2Previous.HasValue)
                {
                    previous = Math.Max(TimestampStartFinish1Previous.Value, TimestampStartFinish2Previous.Value);
                }
                else if (TimestampStartFinish1Previous.HasValue)
                {
                    previous = TimestampStartFinish1Previous.Value;
                }
                else if (TimestampStartFinish2Previous.HasValue)
                {
                    previous = TimestampStartFinish2Previous.Value;
                }

                if (last.HasValue && previous.HasValue)
                {
                    return last.Value - previous.Value;
                }

                return null;
            }
        }

        public int? RefreshRate
        {
            get
            {
                if (TimestampRefreshRateLast.HasValue && TimestampRefreshRatePrevious.HasValue)
                {
                    return (int)(TimestampRefreshRateLast.Value - TimestampRefreshRatePrevious.Value).TotalMilliseconds;
                }
                return null;
            }
        }

        public void Set
        (
            byte packetSequence,
            uint timestampTrack1,
            uint timestampTrack2,
            uint timestampPitlane1,
            uint timestampPitlane2
        )
        {
            PacketSequence = packetSequence;
            if (!TimestampStartFinish1.HasValue || TimestampStartFinish1.Value != timestampTrack1)
            {
                TimestampStartFinish1Previous = TimestampStartFinish1;
                TimestampStartFinish1 = timestampTrack1;
            }
            if (!TimestampStartFinish2.HasValue || TimestampStartFinish2.Value != timestampTrack2)
            {
                TimestampStartFinish2Previous = TimestampStartFinish2;
                TimestampStartFinish2 = timestampTrack2;
            }
            TimestampPitlane1 = timestampPitlane1;
            TimestampPitlane2 = timestampPitlane2;

            TimestampRefreshRatePrevious = TimestampRefreshRateLast;
            TimestampRefreshRateLast = DateTimeOffset.UtcNow;

            _hubContext.Clients.All.ChangedState(this);
        }
    }


    public class ThrottleState
    {
        private readonly IHubContext<Hubs.ThrottleHub, Hubs.IThrottleHub> _hubContext;

        public ThrottleState(IHubContext<Hubs.ThrottleHub, Hubs.IThrottleHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public byte? PacketSequence { get; set; }
        public byte? PicVersion { get; set; }
        public byte? BaseVersion { get; set; }
        public bool? IsDigital { get; set; }
        public byte? Value1 { get; set; }
        public bool? BrakeButton1 { get; set; }
        public bool? LaneChangeButton1 { get; set; }
        public bool? LaneChangeButtonDoubleTapped1 { get; set; }
        public byte? CtrlVersion1 { get; set; }
        public byte? Value2 { get; set; }
        public bool? BrakeButton2 { get; set; }
        public bool? LaneChangeButton2 { get; set; }
        public bool? LaneChangeButtonDoubleTapped2 { get; set; }
        public byte? CtrlVersion2 { get; set; }
        public byte? Value3 { get; set; }
        public bool? BrakeButton3 { get; set; }
        public bool? LaneChangeButton3 { get; set; }
        public bool? LaneChangeButtonDoubleTapped3 { get; set; }
        public byte? CtrlVersion3 { get; set; }
        public byte? Value4 { get; set; }
        public bool? BrakeButton4 { get; set; }
        public bool? LaneChangeButton4 { get; set; }
        public bool? LaneChangeButtonDoubleTapped4 { get; set; }
        public byte? CtrlVersion4 { get; set; }
        public byte? Value5 { get; set; }
        public bool? BrakeButton5 { get; set; }
        public bool? LaneChangeButton5 { get; set; }
        public bool? LaneChangeButtonDoubleTapped5 { get; set; }
        public byte? CtrlVersion5 { get; set; }
        public byte? Value6 { get; set; }
        public bool? BrakeButton6 { get; set; }
        public bool? LaneChangeButton6 { get; set; }
        public bool? LaneChangeButtonDoubleTapped6 { get; set; }
        public byte? CtrlVersion6 { get; set; }
        public uint? Timestamp { get; set; }
        private uint? TimestampPrevious { get; set; }
        private DateTimeOffset? TimeStampRefreshRateLast { get; set; }
        private DateTimeOffset? TimeStampRefreshRatePrevious { get; set; }

        public uint? TimestampInterval
        {
            get
            {
                if (Timestamp.HasValue && TimestampPrevious.HasValue)
                {
                    return Timestamp.Value - TimestampPrevious.Value;
                }
                return null;
            }
        }

        public int? RefreshRate
        {
            get
            {
                if (TimeStampRefreshRateLast.HasValue && TimeStampRefreshRatePrevious.HasValue)
                {
                    return (int)(TimeStampRefreshRateLast.Value - TimeStampRefreshRatePrevious.Value).TotalMilliseconds;
                }
                return null;
            }
        }

        public void Set
        (
            byte packetSequence,
            byte picVersion,
            byte baseVersion,
            bool isDigital,
            byte value1,
            bool brakeButton1,
            bool laneChangeButton1,
            bool laneChangeButtonDoubleTapped1,
            byte ctrlVersion1,
            byte value2,
            bool brakeButton2,
            bool laneChangeButton2,
            bool laneChangeButtonDoubleTapped2,
            byte ctrlVersion2,
            byte value3,
            bool brakeButton3,
            bool laneChangeButton3,
            bool laneChangeButtonDoubleTapped3,
            byte ctrlVersion3,
            byte value4,
            bool brakeButton4,
            bool laneChangeButton4,
            bool laneChangeButtonDoubleTapped4,
            byte ctrlVersion4,
            byte value5,
            bool brakeButton5,
            bool laneChangeButton5,
            bool laneChangeButtonDoubleTapped5,
            byte ctrlVersion5,
            byte value6,
            bool brakeButton6,
            bool laneChangeButton6,
            bool laneChangeButtonDoubleTapped6,
            byte ctrlVersion6,
            uint timestamp
        )
        {
            PacketSequence = packetSequence;
            PicVersion = picVersion;
            BaseVersion = baseVersion;
            IsDigital = isDigital;
            Value1 = value1;
            BrakeButton1 = brakeButton1;
            LaneChangeButton1 = laneChangeButton1;
            LaneChangeButtonDoubleTapped1 = laneChangeButtonDoubleTapped1;
            CtrlVersion1 = ctrlVersion1;
            Value2 = value2;
            BrakeButton2 = brakeButton2;
            LaneChangeButton2 = laneChangeButton2;
            LaneChangeButtonDoubleTapped2 = laneChangeButtonDoubleTapped2;
            CtrlVersion2 = ctrlVersion2;
            Value3 = value3;
            BrakeButton3 = brakeButton3;
            LaneChangeButton3 = laneChangeButton3;
            LaneChangeButtonDoubleTapped3 = laneChangeButtonDoubleTapped3;
            CtrlVersion3 = ctrlVersion3;
            Value4 = value4;
            BrakeButton4 = brakeButton4;
            LaneChangeButton4 = laneChangeButton4;
            LaneChangeButtonDoubleTapped4 = laneChangeButtonDoubleTapped4;
            CtrlVersion4 = ctrlVersion4;
            Value5 = value5;
            BrakeButton5 = brakeButton5;
            LaneChangeButton5 = laneChangeButton5;
            LaneChangeButtonDoubleTapped5 = laneChangeButtonDoubleTapped5;
            CtrlVersion5 = ctrlVersion5;
            Value6 = value6;
            BrakeButton6 = brakeButton6;
            LaneChangeButton6 = laneChangeButton6;
            LaneChangeButtonDoubleTapped6 = laneChangeButtonDoubleTapped6;
            CtrlVersion6 = ctrlVersion6;

            //Console.WriteLine($"{timestamp} {Timestamp} {timestamp - Timestamp}");

            TimestampPrevious = Timestamp;
            Timestamp = timestamp;
            TimeStampRefreshRatePrevious = TimeStampRefreshRateLast;
            TimeStampRefreshRateLast = DateTimeOffset.UtcNow;

            _hubContext.Clients.All.ChangedState(this);
        }
    }
}