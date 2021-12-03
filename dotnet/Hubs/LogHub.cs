using Microsoft.AspNetCore.SignalR;
using ScalextricArcBleProtocolExplorer.Services.MemoryLogger;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Hubs
{
    public interface ILogHub
    {
        Task ChangedState(MemoryLoggerData dto);
    }


    public class LogHub : Hub<ILogHub>
    {
        public void Observe()
        {
        }
    }
}
