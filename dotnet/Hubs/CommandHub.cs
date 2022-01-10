using Microsoft.AspNetCore.SignalR;
using ScalextricArcBleProtocolExplorer.Services.ScalextricArc;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Hubs
{
    public interface ICommandHub
    {
        Task ChangedState(CommandState dto);
    }


    public class CommandHub : Hub<ICommandHub>
    {
        public void Observe()
        {
        }
    }
}
