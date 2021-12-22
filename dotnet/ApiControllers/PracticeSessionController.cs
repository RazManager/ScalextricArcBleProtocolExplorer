using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services.PracticeSession;
using System.Collections.Generic;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/practice-session-car-ids")]
    public class PracticeSessionController : ControllerBase
    {
        private readonly PracticeSessionState _practiceSessionState;


        public PracticeSessionController(PracticeSessionState practiceSessionState)
        {
            _practiceSessionState = practiceSessionState;
        }


        [HttpGet]
        public IEnumerable<PracticeSessionCarIdDto> Get()
        {
            return _practiceSessionState.MapPracticeSessionCarIds();
        }
    }
}
