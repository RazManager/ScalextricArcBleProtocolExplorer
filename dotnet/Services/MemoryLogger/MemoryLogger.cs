using Microsoft.Extensions.Logging;
using System;


namespace ScalextricArcBleProtocolExplorer.Services.MemoryLogger
{
    public class MemoryLogger : ILogger
    {
        private readonly string _category;


        public MemoryLogger(string category)
        {
            _category = category;
        }


        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }


        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (exception != null)
            {
                MemoryLoggerChannel.ChannelData.Writer.TryWrite(new MemoryLoggerData(_category, logLevel, formatter(state, exception)));
            }
            else
            {
                if (state is null)
                {
                    MemoryLoggerChannel.ChannelData.Writer.TryWrite(new MemoryLoggerData(_category, logLevel, "?"));
                }
                else
                {
                    MemoryLoggerChannel.ChannelData.Writer.TryWrite(new MemoryLoggerData(_category, logLevel, state.ToString()!));
                }
            }
        }
    }
}
