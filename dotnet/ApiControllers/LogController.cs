using Microsoft.AspNetCore.Mvc;
using ScalextricArcBleProtocolExplorer.Services.MemoryLogger;
using System.Collections.Generic;


namespace ScalextricArcBleProtocolExplorer.ApiControllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogController : ControllerBase
    {
        private readonly Queue<MemoryLoggerData> _queue;


        public LogController(Queue<MemoryLoggerData> queue)
        {
            _queue = queue;
        }


        [HttpGet]
        public MemoryLoggerData[] Get()
        {
            return _queue.ToArray();
        }
    }
}
