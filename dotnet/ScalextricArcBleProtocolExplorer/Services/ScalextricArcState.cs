using System;


namespace ScalextricArcBleProtocolExplorer.Services
{
    public class ScalextricArcState
    {
        public byte? ThrottlePacketSequence { get; private set; }
        public byte? ThrottleValue1 { get; private set; }
        public bool? ThrottleValueBrakeButton1 { get; private set; }
        public bool? ThrottleValueLaneChangeButton1 { get; private set; }
        public byte? ThrottleValue2 { get; private set; }
        public bool? ThrottleValueBrakeButton2 { get; private set; }
        public bool? ThrottleValueLaneChangeButton2 { get; private set; }
        public byte? ThrottleValue3 { get; private set; }
        public bool? ThrottleValueBrakeButton3 { get; private set; }
        public bool? ThrottleValueLaneChangeButton3 { get; private set; }
        public byte? ThrottleValue4 { get; private set; }
        public bool? ThrottleValueBrakeButton4 { get; private set; }
        public bool? ThrottleValueLaneChangeButton4 { get; private set; }
        public byte? ThrottleValue5 { get; private set; }
        public bool? ThrottleValueBrakeButton5 { get; private set; }
        public bool? ThrottleValueLaneChangeButton5 { get; private set; }
        public byte? ThrottleValue6 { get; private set; }
        public bool? ThrottleValueBrakeButton6 { get; private set; }
        public bool? ThrottleValueLaneChangeButton6 { get; private set; }
        public uint? ThrottleTimestamp { get; private set; }
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
            byte throttleValue1,
            bool throttleValueBrakeButton1,
            bool throttleValueLaneChangeButton1,
            byte throttleValue2,
            bool throttleValueBrakeButton2,
            bool throttleValueLaneChangeButton2,
            byte throttleValue3,
            bool throttleValueBrakeButton3,
            bool throttleValueLaneChangeButton3,
            byte throttleValue4,
            bool throttleValueBrakeButton4,
            bool throttleValueLaneChangeButton4,
            byte throttleValue5,
            bool throttleValueBrakeButton5,
            bool throttleValueLaneChangeButton5,
            byte throttleValue6,
            bool throttleValueBrakeButton6,
            bool throttleValueLaneChangeButton6,
            uint throttleTimestamp
        )
        {
            ThrottlePacketSequence = throttlePacketSequence;
            ThrottleValue1 = throttleValue1;
            ThrottleValueBrakeButton1 = throttleValueBrakeButton1;
            ThrottleValueLaneChangeButton1 = throttleValueLaneChangeButton1;
            ThrottleValue2 = throttleValue2;
            ThrottleValueBrakeButton2 = throttleValueBrakeButton2;
            ThrottleValueLaneChangeButton2 = throttleValueLaneChangeButton2;
            ThrottleValue3 = throttleValue3;
            ThrottleValueBrakeButton3 = throttleValueBrakeButton3;
            ThrottleValueLaneChangeButton3 = throttleValueLaneChangeButton3;
            ThrottleValue4 = throttleValue4;
            ThrottleValueBrakeButton4 = throttleValueBrakeButton4;
            ThrottleValueLaneChangeButton4 = throttleValueLaneChangeButton4;
            ThrottleValue5 = throttleValue5;
            ThrottleValueBrakeButton5 = throttleValueBrakeButton5;
            ThrottleValueLaneChangeButton5 = throttleValueLaneChangeButton5;
            ThrottleValue6 = throttleValue6;
            ThrottleValueBrakeButton6 = throttleValueBrakeButton6;
            ThrottleValueLaneChangeButton6 = throttleValueLaneChangeButton6;
            ThrottleTimestamp = throttleTimestamp;

            ThrottleTimeStampServicePrevious = ThrottleTimeStampServiceLast;
            ThrottleTimeStampServiceLast = DateTimeOffset.UtcNow;
        }
    }
}
