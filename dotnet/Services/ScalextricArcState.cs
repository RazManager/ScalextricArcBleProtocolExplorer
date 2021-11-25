using Microsoft.AspNetCore.SignalR;
using System;


namespace ScalextricArcBleProtocolExplorer.Services
{
    public class ScalextricArcState
    {
        public DeviceState DeviceState { get; init; } = new();
        public ThrottleState ThrottleState { get; init; }
        public SlotState[] SlotStates { get; init; } = new SlotState[6];

        public ScalextricArcState(IHubContext<Hubs.ThrottleHub, Hubs.IThrottleHub> throttleHubContext)
        {
            ThrottleState = new ThrottleState(throttleHubContext);

            for (int i = 0; i < SlotStates.Length; i++)
            {
                SlotStates[i] = new SlotState();
            }
        }
    }
    

    public class DeviceState
    {
        public string? ManufacturerName { get; set; }
        public string? ModelNumber { get; set; }
        public string? HardwareRevision { get; set; }
        public string? FirmwareRevision { get; set; }
        public string? SoftwareRevision { get; set; }

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
    }


    public class ThrottleState
    {
        private readonly IHubContext<Hubs.ThrottleHub, Hubs.IThrottleHub> _throttleHubContext;

        public ThrottleState(IHubContext<Hubs.ThrottleHub, Hubs.IThrottleHub> throttleHubContext)
        {
            _throttleHubContext = throttleHubContext;
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
        private DateTimeOffset? TimeStampServicePrevious { get; set; }
        private DateTimeOffset? TimeStampService { get; set; }

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

        public int? TimestampServiceInterval
        {
            get
            {
                if (TimeStampService.HasValue && TimeStampServicePrevious.HasValue)
                {
                    return (int)(TimeStampService.Value - TimeStampServicePrevious.Value).TotalMilliseconds;
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
            TimestampPrevious = Timestamp;
            Timestamp = timestamp;
            TimeStampServicePrevious = TimeStampService;
            TimeStampService = DateTimeOffset.UtcNow;

            _throttleHubContext.Clients.All.ChangedState(this).Wait();
        }
    }


    public class SlotState
    {
        public byte? PacketSequence { get; set; }
        public byte? CarId { get; set; }
        public uint? TimestampTrack1 { get; set; }
        private uint? TimestampTrack1Previous { get; set; }
        public uint? TimestampTrack2 { get; set; }
        private uint? TimestampTrack2Previous { get; set; }
        public uint? TimestampPitlane1 { get; set; }
        private uint? TimestampPitlane1Previous { get; set; }
        public uint? TimestampPitlane2 { get; set; }
        private uint? TimestampPitlane2Previous { get; set; }

        public uint? TimestampTrack1Interval
        {
            get
            {
                if (TimestampTrack1.HasValue && TimestampTrack1Previous.HasValue)
                {
                    return TimestampTrack1.Value - TimestampTrack1Previous.Value;
                }
                return null;
            }
        }

        public uint? TimestampTrack2Interval
        {
            get
            {
                if (TimestampTrack2.HasValue && TimestampTrack2Previous.HasValue)
                {
                    return TimestampTrack2.Value - TimestampTrack2Previous.Value;
                }
                return null;
            }
        }

        public uint? TimestampPitlane1Interval
        {
            get
            {
                if (TimestampPitlane1.HasValue && TimestampPitlane1Previous.HasValue)
                {
                    return TimestampPitlane1.Value - TimestampPitlane1Previous.Value;
                }
                return null;
            }
        }

        public uint? TimestampPitlane2Interval
        {
            get
            {
                if (TimestampPitlane2.HasValue && TimestampPitlane2Previous.HasValue)
                {
                    return TimestampPitlane2.Value - TimestampPitlane2Previous.Value;
                }
                return null;
            }
        }

        public void Set
        (
            byte packetSequence,
            byte carId,
            uint timestampTrack1,
            uint timestampTrack2,
            uint timestampPitlane1,
            uint timestampPitlane2
        )
        {
            PacketSequence = packetSequence;
            CarId = carId;
            TimestampTrack1Previous = TimestampTrack1;
            TimestampTrack1 = timestampTrack1;
            TimestampTrack2Previous = TimestampTrack2;
            TimestampTrack2 = timestampTrack2;
            TimestampPitlane1Previous = TimestampPitlane1;
            TimestampPitlane1 = timestampPitlane1;
            TimestampPitlane2Previous = TimestampPitlane2;
            TimestampPitlane2 = timestampPitlane2;
        }
    }
}