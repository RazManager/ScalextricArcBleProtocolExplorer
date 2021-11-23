using System;


namespace ScalextricArcBleProtocolExplorer.Services
{
    public class ScalextricArcState
    {
        public ThrottleState ThrottleState { get; init; } = new();
        public SlotState[] SlotStates { get; init; } = new SlotState[6];
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

        public uint? ThrottleTimestampDuration
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

        public int? ThrottleTimestampServiceDuration
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
            ThrottleTimestamp = throttleTimestamp;

            ThrottleTimeStampServicePrevious = ThrottleTimeStampServiceLast;
            ThrottleTimeStampServiceLast = DateTimeOffset.UtcNow;
        }
    }



    public class SlotState
    {
        private byte? SlotPacketSequence { get; set; }
        private byte? SlotCarId { get; set; }
        private uint? SlotTimestampTrack1 { get; set; }
        private uint? SlotTimestampTrack2 { get; set; }
        private uint? SlotTimestampPitlane1 { get; set; }
        private uint? SlotTimestampPitlane2 { get; set; }
    }

}





