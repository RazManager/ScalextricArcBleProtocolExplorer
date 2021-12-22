using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services.ScalextricArc;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/slots")]
    public class SlotController : ControllerBase
    {
        private readonly ScalextricArcState _scalextricArcState;


        public SlotController(ScalextricArcState scalextricArcState)
        {
            _scalextricArcState = scalextricArcState;
        }


        [HttpGet]
        public SlotState[] Get()
        {
            return _scalextricArcState.SlotStates;
        }
    }
}
