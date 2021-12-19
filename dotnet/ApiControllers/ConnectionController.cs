using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services;
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
        public ConnectionState Get()
        {
            return _scalextricArcState.ConnectionState;
        }


        [HttpPut]
        public Task PostAsync([FromBody][Required] ConnectionDto dto)
        {
            return _scalextricArcState.ConnectionState.SetConnectAsync(dto.Connect);
        }
    }
}
