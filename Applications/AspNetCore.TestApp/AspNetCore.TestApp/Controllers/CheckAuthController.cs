using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text;

namespace AspNetCore.TestApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CheckAuthController : Controller
    {
        [HttpGet("getRequestInfo")]
        public ActionResult<string> GetRequestInfo()
        {
            var result = new StringBuilder();
            result.AppendLine("Query:");
            var queryParams = HttpContext.Request.Query.Select(param => $"{param.Key} : {param.Value}");
            result.AppendLine(string.Join(Environment.NewLine, queryParams));

            result.AppendLine("Headers:");
            var headers = HttpContext.Request.Headers.Select(header => $"{header.Key} : {header.Value}");
            result.AppendLine(string.Join(Environment.NewLine, headers));

            return result.ToString();
        }
    }
}
