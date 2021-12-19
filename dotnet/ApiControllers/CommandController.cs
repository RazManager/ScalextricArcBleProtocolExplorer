using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/command")]
    public class CommandController : ControllerBase
    {
        private readonly ScalextricArcState _scalextricArcState;


        public CommandController(ScalextricArcState scalextricArcState)
        {
            _scalextricArcState = scalextricArcState;
        }


        [HttpGet]
        public CommandState Get()
        {
            return _scalextricArcState.CommandState;
        }


        [HttpPut]
        public Task PostAsync([FromBody][Required] CommandDto dto)
        {
            return _scalextricArcState.CommandState.SetAsync
            (
                dto.Command,
                dto.PowerMultiplier1,
                dto.Ghost1,
                dto.Rumble1,
                dto.Brake1,
                dto.Kers1,
                dto.PowerMultiplier2,
                dto.Ghost2,
                dto.Rumble2,
                dto.Brake2,
                dto.Kers2,
                dto.PowerMultiplier3,
                dto.Ghost3,
                dto.Rumble3,
                dto.Brake3,
                dto.Kers3,
                dto.PowerMultiplier4,
                dto.Ghost4,
                dto.Rumble4,
                dto.Brake4,
                dto.Kers4,
                dto.PowerMultiplier5,
                dto.Ghost5,
                dto.Rumble5,
                dto.Brake5,
                dto.Kers5,
                dto.PowerMultiplier6,
                dto.Ghost6,
                dto.Rumble6,
                dto.Brake6,
                dto.Kers6,
                true
            );
        }
    }
}
