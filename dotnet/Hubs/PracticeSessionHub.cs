using Microsoft.AspNetCore.SignalR;
using ScalextricArcBleProtocolExplorer.Services;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Hubs
{
    public interface IPracticeSessionHub
    {
        Task ChangedState(PracticeSessionCarId dto);
    }


    public class PracticeSessionHub : Hub<IPracticeSessionHub>
    {
        public void Observe()
        {
        }
    }
}
