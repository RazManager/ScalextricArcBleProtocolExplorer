using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using ScalextricArcBleProtocolExplorer.Hubs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Services.MemoryLogger
{
    public class MemoryLoggerService : BackgroundService
    {
        private readonly Queue<MemoryLoggerData> _queue;
        private readonly IHubContext<LogHub, ILogHub> _hubContext;

        public MemoryLoggerService(Queue<MemoryLoggerData> queue,
                                   IHubContext<Hubs.LogHub, Hubs.ILogHub> hubContext)
        {
            _queue = queue;
            _hubContext = hubContext;
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await foreach (var memoryLoggerData in MemoryLoggerChannel.ChannelData.Reader.ReadAllAsync(cancellationToken))
            {
                _queue.Enqueue(memoryLoggerData);
                await _hubContext.Clients.All.ChangedState(memoryLoggerData);
                while (_queue.Count > 1000)
                {
                    _queue.Dequeue();
                }
            }
        }
    }
}