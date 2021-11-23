using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/throttle")]
    public class ThrottleController : ControllerBase
    {
        private readonly ScalextricArcState _scalextricArcState;


        public ThrottleController(ScalextricArcState scalextricArcState)
        {
            _scalextricArcState = scalextricArcState;
        }


        [HttpGet]
        public ThrottleState Get()
        {
            return _scalextricArcState.ThrottleState;
        }
    }
}
