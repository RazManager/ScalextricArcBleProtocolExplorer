using Microsoft.AspNetCore.SignalR;
using ScalextricArcBleProtocolExplorer.Services;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Hubs
{
    public interface ITrackHub
    {
        Task ChangedState(TrackState dto);
    }


    public class TrackHub : Hub<ITrackHub>
    {
        public void Observe()
        {
        }
    }
}
