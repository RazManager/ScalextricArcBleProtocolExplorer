using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/device")]
    public class DeviceController : ControllerBase
    {
        private readonly ScalextricArcState _scalextricArcState;


        public DeviceController(ScalextricArcState scalextricArcState)
        {
            _scalextricArcState = scalextricArcState;
        }


        [HttpGet]
        public DeviceState Get()
        {
            return _scalextricArcState.DeviceState;
        }
    }
}
