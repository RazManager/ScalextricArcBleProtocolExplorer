using Microsoft.AspNetCore.SignalR;
using ScalextricArcBleProtocolExplorer.Services.ScalextricArc;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Hubs
{
    public interface IThrottleProfileHub
    {
        Task ChangedState(ThrottleProfileState dto);
    }


    public class ThrottleProfileHub : Hub<IThrottleProfileHub>
    {
        public void Observe()
        {
        }
    }
}
