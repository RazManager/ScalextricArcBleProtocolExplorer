using System;


namespace ScalextricArcBleProtocolExplorer.Services
{
    public class ScalextricArcState
    {
        public DeviceState DeviceState { get; init; } = new();
        public ThrottleState ThrottleState { get; init; } = new();
        public SlotState[] SlotStates { get; init; } = new SlotState[6];
    }


    public class DeviceState
    {
        public string? ManufacturerName { get; set; }
        public string? ModelNumber { get; set; }
        public string? HardwareRevision { get; set; }
        public string? FirmwareRevision { get; set; }
        public string? SoftwareRevision { get; set; }
        public string? NordicDfuRevision { get; set; }

        public void SetDeviceState
        (
            string? manufacturerName,
            string? modelNumber,
            string? hardwareRevision,
            string? firmwareRevision,
            string? softwareRevision,
            string? nordicDfuRevision
        )
        {
            ManufacturerName = manufacturerName;
            ModelNumber = modelNumber;
            HardwareRevision = hardwareRevision;
            FirmwareRevision = firmwareRevision;
            SoftwareRevision = softwareRevision;
            NordicDfuRevision = nordicDfuRevision;
        }
    }


    public class ThrottleState
    {
        public byte? ThrottlePacketSequence { get; set; }
        public byte? ThrottlePicVersion { get; set; }
        public byte? ThrottleBaseVersion { get; set; }
        public bool? ThrottleIsDigital { get; set; }
        public byte? ThrottleValue1 { get; set; }
        public bool? ThrottleValueBrakeButton1 { get; set; }
        public bool? ThrottleValueLaneChangeButton1 { get; set; }
        public bool? ThrottleValueLaneChangeButtonDoubleTapped1 { get; set; }
        public byte? ThrottleCtrlVersion1 { get; set; }
        public byte? ThrottleValue2 { get; set; }
        public bool? ThrottleValueBrakeButton2 { get; set; }
        public bool? ThrottleValueLaneChangeButton2 { get; set; }
        public bool? ThrottleValueLaneChangeButtonDoubleTapped2 { get; set; }
        public byte? ThrottleCtrlVersion2 { get; set; }
        public byte? ThrottleValue3 { get; set; }
        public bool? ThrottleValueBrakeButton3 { get; set; }
        public bool? ThrottleValueLaneChangeButton3 { get; set; }
        public bool? ThrottleValueLaneChangeButtonDoubleTapped3 { get; set; }
        public byte? ThrottleCtrlVersion3 { get; set; }
        public byte? ThrottleValue4 { get; set; }
        public bool? ThrottleValueBrakeButton4 { get; set; }
        public bool? ThrottleValueLaneChangeButton4 { get; set; }
        public bool? ThrottleValueLaneChangeButtonDoubleTapped4 { get; set; }
        public byte? ThrottleCtrlVersion4 { get; set; }
        public byte? ThrottleValue5 { get; set; }
        public bool? ThrottleValueBrakeButton5 { get; set; }
        public bool? ThrottleValueLaneChangeButton5 { get; set; }
        public bool? ThrottleValueLaneChangeButtonDoubleTapped5 { get; set; }
        public byte? ThrottleCtrlVersion5 { get; set; }
        public byte? ThrottleValue6 { get; set; }
        public bool? ThrottleValueBrakeButton6 { get; set; }
        public bool? ThrottleValueLaneChangeButton6 { get; set; }
        public bool? ThrottleValueLaneChangeButtonDoubleTapped6 { get; set; }
        public byte? ThrottleCtrlVersion6 { get; set; }
        public uint? ThrottleTimestamp { get; set; }
        private uint? ThrottleTimestampPrevious { get; set; }
        private DateTimeOffset? ThrottleTimeStampServicePrevious { get; set; }
        private DateTimeOffset? ThrottleTimeStampServiceLast { get; set; }

        public uint? ThrottleTimestampInterval
        {
            get
            {
                if (ThrottleTimestamp.HasValue && ThrottleTimestampPrevious.HasValue)
                {
                    return ThrottleTimestamp.Value - ThrottleTimestampPrevious.Value;
                }
                return null;
            }
        }

        public int? ThrottleTimestampServiceInterval
        {
            get
            {
                if (ThrottleTimeStampServiceLast.HasValue && ThrottleTimeStampServicePrevious.HasValue)
                {
                    return (int)(ThrottleTimeStampServiceLast.Value - ThrottleTimeStampServicePrevious.Value).TotalMilliseconds;
                }
                return null;
            }
        }

        public void SetThrottleState
        (
            byte throttlePacketSequence,
            byte throttlePicVersion,
            byte throttleBaseVersion,
            bool throttleIsDigital,
            byte throttleValue1,
            bool throttleValueBrakeButton1,
            bool throttleValueLaneChangeButton1,
            bool throttleValueLaneChangeButtonDoubleTapped1,
            byte throttleCtrlVersion1,
            byte throttleValue2,
            bool throttleValueBrakeButton2,
            bool throttleValueLaneChangeButton2,
            bool throttleValueLaneChangeButtonDoubleTapped2,
            byte throttleCtrlVersion2,
            byte throttleValue3,
            bool throttleValueBrakeButton3,
            bool throttleValueLaneChangeButton3,
            bool throttleValueLaneChangeButtonDoubleTapped3,
            byte throttleCtrlVersion3,
            byte throttleValue4,
            bool throttleValueBrakeButton4,
            bool throttleValueLaneChangeButton4,
            bool throttleValueLaneChangeButtonDoubleTapped4,
            byte throttleCtrlVersion4,
            byte throttleValue5,
            bool throttleValueBrakeButton5,
            bool throttleValueLaneChangeButton5,
            bool throttleValueLaneChangeButtonDoubleTapped5,
            byte throttleCtrlVersion5,
            byte throttleValue6,
            bool throttleValueBrakeButton6,
            bool throttleValueLaneChangeButton6,
            bool throttleValueLaneChangeButtonDoubleTapped6,
            byte throttleCtrlVersion6,
            uint throttleTimestamp
        )
        {
            ThrottlePacketSequence = throttlePacketSequence;
            ThrottlePicVersion = throttlePicVersion;
            ThrottleBaseVersion = throttleBaseVersion;
            ThrottleIsDigital = throttleIsDigital;
            ThrottleValue1 = throttleValue1;
            ThrottleValueBrakeButton1 = throttleValueBrakeButton1;
            ThrottleValueLaneChangeButton1 = throttleValueLaneChangeButton1;
            ThrottleValueLaneChangeButtonDoubleTapped1 = throttleValueLaneChangeButtonDoubleTapped1;
            ThrottleCtrlVersion1 = throttleCtrlVersion1;
            ThrottleValue2 = throttleValue2;
            ThrottleValueBrakeButton2 = throttleValueBrakeButton2;
            ThrottleValueLaneChangeButton2 = throttleValueLaneChangeButton2;
            ThrottleValueLaneChangeButtonDoubleTapped2 = throttleValueLaneChangeButtonDoubleTapped2;
            ThrottleCtrlVersion2 = throttleCtrlVersion2;
            ThrottleValue3 = throttleValue3;
            ThrottleValueBrakeButton3 = throttleValueBrakeButton3;
            ThrottleValueLaneChangeButton3 = throttleValueLaneChangeButton3;
            ThrottleValueLaneChangeButtonDoubleTapped3 = throttleValueLaneChangeButtonDoubleTapped3;
            ThrottleCtrlVersion3 = throttleCtrlVersion3;
            ThrottleValue4 = throttleValue4;
            ThrottleValueBrakeButton4 = throttleValueBrakeButton4;
            ThrottleValueLaneChangeButton4 = throttleValueLaneChangeButton4;
            ThrottleValueLaneChangeButtonDoubleTapped4 = throttleValueLaneChangeButtonDoubleTapped4;
            ThrottleCtrlVersion4 = throttleCtrlVersion4;
            ThrottleValue5 = throttleValue5;
            ThrottleValueBrakeButton5 = throttleValueBrakeButton5;
            ThrottleValueLaneChangeButton5 = throttleValueLaneChangeButton5;
            ThrottleValueLaneChangeButtonDoubleTapped5 = throttleValueLaneChangeButtonDoubleTapped5;
            ThrottleCtrlVersion5 = throttleCtrlVersion5;
            ThrottleValue6 = throttleValue6;
            ThrottleValueBrakeButton6 = throttleValueBrakeButton6;
            ThrottleValueLaneChangeButton6 = throttleValueLaneChangeButton6;
            ThrottleValueLaneChangeButtonDoubleTapped6 = throttleValueLaneChangeButtonDoubleTapped6;
            ThrottleCtrlVersion6 = throttleCtrlVersion6;
            ThrottleTimestampPrevious = ThrottleTimestamp;
            ThrottleTimestamp = throttleTimestamp;

            ThrottleTimeStampServicePrevious = ThrottleTimeStampServiceLast;
            ThrottleTimeStampServiceLast = DateTimeOffset.UtcNow;
        }
    }


    public class SlotState
    {
        public byte? SlotPacketSequence { get; set; }
        public byte? SlotCarId { get; set; }
        public uint? SlotTimestampTrack1 { get; set; }
        private uint? SlotTimestampTrack1Previous { get; set; }
        public uint? SlotTimestampTrack2 { get; set; }
        private uint? SlotTimestampTrack2Previous { get; set; }
        public uint? SlotTimestampPitlane1 { get; set; }
        private uint? SlotTimestampPitlane1Previous { get; set; }
        public uint? SlotTimestampPitlane2 { get; set; }
        private uint? SlotTimestampPitlane2Previous { get; set; }

        public uint? SlotTimestampTrack1Interval
        {
            get
            {
                if (SlotTimestampTrack1.HasValue && SlotTimestampTrack1Previous.HasValue)
                {
                    return SlotTimestampTrack1.Value - SlotTimestampTrack1Previous.Value;
                }
                return null;
            }
        }

        public uint? SlotTimestampTrack2Interval
        {
            get
            {
                if (SlotTimestampTrack2.HasValue && SlotTimestampTrack2Previous.HasValue)
                {
                    return SlotTimestampTrack2.Value - SlotTimestampTrack2Previous.Value;
                }
                return null;
            }
        }

        public uint? SlotTimestampPitlane1Interval
        {
            get
            {
                if (SlotTimestampPitlane1.HasValue && SlotTimestampPitlane1Previous.HasValue)
                {
                    return SlotTimestampPitlane1.Value - SlotTimestampPitlane1Previous.Value;
                }
                return null;
            }
        }

        public uint? SlotTimestampPitlane2Interval
        {
            get
            {
                if (SlotTimestampPitlane2.HasValue && SlotTimestampPitlane2Previous.HasValue)
                {
                    return SlotTimestampPitlane2.Value - SlotTimestampPitlane2Previous.Value;
                }
                return null;
            }
        }

        public void SetSlotState
        (
            byte slotPacketSequence,
            byte slotCarId,
            uint slotTimestampTrack1,
            uint slotTimestampTrack2,
            uint slotTimestampPitlane1,
            uint slotTimestampPitlane2
        )
        {
            SlotPacketSequence = slotPacketSequence;
            SlotCarId = slotCarId;
            SlotTimestampTrack1Previous = SlotTimestampTrack1;
            SlotTimestampTrack1 = slotTimestampTrack1;
            SlotTimestampTrack2Previous = SlotTimestampTrack2;
            SlotTimestampTrack2 = slotTimestampTrack2;
            SlotTimestampPitlane1Previous = SlotTimestampPitlane1;
            SlotTimestampPitlane1 = slotTimestampPitlane1;
            SlotTimestampPitlane2Previous = SlotTimestampPitlane2;
            SlotTimestampPitlane2 = slotTimestampPitlane2;
        }
    }
}