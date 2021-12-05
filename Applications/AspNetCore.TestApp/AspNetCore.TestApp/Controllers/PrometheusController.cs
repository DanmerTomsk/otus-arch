using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.TestApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PrometheusController : ControllerBase
    {
        private static int _requestAttempt = 0;
        private static SemaphoreSlim _requestAttemptSemaphore = new SemaphoreSlim(1);

        [HttpGet("failByAttempt")]
        public async Task<ActionResult<int>> FailByAttempt([FromQuery(Name = "attempt")] int expectedAttemptCount = 3)
        {
            await _requestAttemptSemaphore.WaitAsync(HttpContext.RequestAborted);
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
                _requestAttemptSemaphore.Release();
            }
        }
    }
}
