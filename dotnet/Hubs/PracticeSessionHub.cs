﻿using Microsoft.AspNetCore.SignalR;
using ScalextricArcBleProtocolExplorer.Services.PracticeSession;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Hubs
{
    public interface IPracticeSessionHub
    {
        Task ChangedState(PracticeSessionCarIdDto dto);
    }


    public class PracticeSessionHub : Hub<IPracticeSessionHub>
    {
        public void Observe()
        {
        }
    }
}
