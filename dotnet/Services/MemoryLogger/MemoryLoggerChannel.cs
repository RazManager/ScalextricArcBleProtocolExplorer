using System.Threading.Channels;


namespace ScalextricArcBleProtocolExplorer.Services.MemoryLogger
{
    public class MemoryLoggerChannel
    {
        public static Channel<MemoryLoggerData> ChannelData = Channel.CreateBounded<MemoryLoggerData>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleWriter = false,
            SingleReader = true
        });
    }
}
