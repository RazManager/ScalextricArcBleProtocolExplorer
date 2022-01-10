using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services.ScalextricArc;
using System.Collections.Generic;
using System.Linq;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/gatt-characteristics")]
    public class GattCharacteristicController : ControllerBase
    {
        private readonly ScalextricArcState _scalextricArcState;


        public GattCharacteristicController(ScalextricArcState scalextricArcState)
        {
            _scalextricArcState = scalextricArcState;
        }


        [HttpGet]
        public IEnumerable<GattCharacteristic> Get()
        {
            return _scalextricArcState.GattCharacteristics.OrderBy(x => x.uuid);
        }
    }
}
