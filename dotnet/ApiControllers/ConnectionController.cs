using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services.ScalextricArc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


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
        public ConnectionDto Get()
        {
            return _scalextricArcState.ConnectionState.MapDto();
        }


        [HttpPut]
        public Task PostAsync([FromBody][Required] ConnectDto dto)
        {
            return _scalextricArcState.ConnectionState.SetConnectAsync(dto.Connect);
        }
    }
}
