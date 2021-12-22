using Microsoft.AspNetCore.SignalR;
using ScalextricArcBleProtocolExplorer.Services.ScalextricArc;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Hubs
{
    public interface IThrottleHub
    {
        Task ChangedState(ThrottleState dto);
    }


    public class ThrottleHub : Hub<IThrottleHub>
    {
        public void Observe()
        {
        }
    }
}
