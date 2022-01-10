using Microsoft.Extensions.Logging;
using System;


namespace ScalextricArcBleProtocolExplorer.Services.MemoryLogger
{
    public class MemoryLoggerData
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Category { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }

        public MemoryLoggerData(string category, LogLevel logLevel, string message)
        {
            Timestamp = DateTimeOffset.UtcNow;
            Category = category;
            LogLevel = logLevel;
            Message = message;
        }
    }
}
