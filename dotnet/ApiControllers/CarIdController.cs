using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services.ScalextricArc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/car-id")]
    public class CarIdController : ControllerBase
    {
        private readonly ScalextricArcState _scalextricArcState;


        public CarIdController(ScalextricArcState scalextricArcState)
        {
            _scalextricArcState = scalextricArcState;
        }


        [HttpGet]
        public CarIdState Get()
        {
            return _scalextricArcState.CarIdState;
        }


        [HttpPut]
        public Task PostAsync([FromBody][Required] CarIdDto dto)
        {
            return _scalextricArcState.CarIdState.SetAsync
            (
                dto.CarId,
                true
            );
        }
    }
}
