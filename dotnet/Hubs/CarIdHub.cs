﻿using Microsoft.AspNetCore.SignalR;
using ScalextricArcBleProtocolExplorer.Services.ScalextricArc;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.Hubs
{
    public interface ICarIdHub
    {
        Task ChangedState(CarIdState dto);
    }


    public class CarIdHub : Hub<ICarIdHub>
    {
        public void Observe()
        {
        }
    }
}
