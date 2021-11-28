using Microsoft.AspNetCore.SignalR;
using ScalextricArcBleProtocolExplorer.Services;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Hubs
{
    public interface IConnectionHub
    {
        Task ChangedState(ConnectionState dto);
    }


    public class ConnectionHub : Hub<IConnectionHub>
    {
        public void Observe()
        {
        }
    }
}
