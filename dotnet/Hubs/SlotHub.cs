using Microsoft.AspNetCore.SignalR;
using ScalextricArcBleProtocolExplorer.Services;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Hubs
{
    public interface ISlotHub
    {
        Task ChangedState(SlotState dto);
    }


    public class SlotHub : Hub<ISlotHub>
    {
        public void Observe()
        {
        }
    }
}
