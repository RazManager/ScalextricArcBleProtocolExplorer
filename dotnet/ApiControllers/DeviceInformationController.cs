using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/device-information")]
    public class DeviceInformationController : ControllerBase
    {
        private readonly ScalextricArcState _scalextricArcState;


        public DeviceInformationController(ScalextricArcState scalextricArcState)
        {
            _scalextricArcState = scalextricArcState;
        }


        [HttpGet]
        public DeviceInformation Get()
        {
            return _scalextricArcState.DeviceInformation;
        }
    }
}
