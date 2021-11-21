using AspNetCore.TestApp.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.TestApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private UserDbContext _userDbContext;

        public DebugController(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        [HttpGet("connstring")]
        public ActionResult<string> GetConnString()
        {
            return Ok(_userDbContext.Database.GetConnectionString());
        }
    }
}
