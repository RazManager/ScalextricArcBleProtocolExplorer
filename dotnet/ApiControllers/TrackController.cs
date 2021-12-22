using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services.ScalextricArc;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/track")]
    public class TrackController : ControllerBase
    {
        private readonly ScalextricArcState _scalextricArcState;


        public TrackController(ScalextricArcState scalextricArcState)
        {
            _scalextricArcState = scalextricArcState;
        }


        [HttpGet]
        public TrackState Get()
        {
            return _scalextricArcState.TrackState;
        }
    }
}
