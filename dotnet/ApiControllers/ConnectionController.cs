using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/connection")]
    public class ConnectionController : ControllerBase
    {
        private readonly ScalextricArcState _scalextricArcState;


        public ConnectionController(ScalextricArcState scalextricArcState)
        {
            _scalextricArcState = scalextricArcState;
        }


        [HttpGet]
        public ConnectionState Get()
        {
            return _scalextricArcState.ConnectionState;
        }
    }
}
