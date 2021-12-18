using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.TestApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PrometheusController : ControllerBase
    {
        private static int _requestAttempt = 0;
        private static Random _random = new Random(DateTime.Now.Millisecond);

        [HttpGet("failByAttempt")]
        public async Task<ActionResult<int>> FailByAttempt(
            [FromQuery(Name = "attempt")] int expectedAttemptCount = 3,
            [FromQuery(Name = "useLatency")] bool useRandomLatency = false)
        {
            if (useRandomLatency)
            { 
                var delayMs = _random.Next(1000);
                await Task.Delay(TimeSpan.FromMilliseconds(delayMs));
            }

            try
            {
                _requestAttempt++;
                if (_requestAttempt >= expectedAttemptCount)
                {
                    _requestAttempt = 0;
                    return Problem(title: "Expected attempt has been achieved");
                }
                return Ok(_requestAttempt);
            }
            finally
            {
            }
        }
    }
}
