using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/throttle-profiles")]
    public class ThrottleProfileController : ControllerBase
    {
        private readonly ScalextricArcState _scalextricArcState;


        public ThrottleProfileController(ScalextricArcState scalextricArcState)
        {
            _scalextricArcState = scalextricArcState;
        }


        [HttpGet]
        public ThrottleProfileState[] Get()
        {
            return _scalextricArcState.ThrottleProfileStates;
        }


        [HttpPost]
        public Task PostAsync([FromBody][Required] ThrottleProfileDto dto)
        {
            return _scalextricArcState.ThrottleProfileStates[dto.CarId - 1].SetAsync(dto.Values);
        }
    }
}
