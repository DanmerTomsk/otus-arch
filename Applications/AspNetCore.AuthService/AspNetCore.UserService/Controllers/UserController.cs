using AspNetCore.Common;
using AspNetCore.Common.Db;
using AspNetCore.Common.Helpers;
using AspNetCore.Common.Models;
using AspNetCore.UserService.Helpers;
using AspNetCore.UserService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly AccountsDbContext _authorizationDbContext;

        public UserController(AccountsDbContext authorizationDbContext)
        {
            _authorizationDbContext = authorizationDbContext;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            if (User.IsInRole(Constants.AdminRoleName))
            {
                var users = await _authorizationDbContext.Users.ToArrayAsync(HttpContext.RequestAborted);
                return Ok(users);
            }

            if (User.Identity is null)
            {
                return Problem("Can't get identity for user. Relogin please");
            }

            var user = await _authorizationDbContext.Users.Include(user => user.Role).FirstOrDefaultAsync(user => user.Username == User.Identity.Name);

            return Ok(user);
        }

        // GET user/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<DbUser>> GetById(int id)
        {
            try
            {
                var user = await _authorizationDbContext.Users.FindAsync(id);

                if (user is null)
                {
                    return NotFound($"Can't get user by [{id}] id");
                }

                if (!IsRequestedUserAutorizedOrAdmin(user))
                {
                    return Forbid();
                }

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't get data from DB");
            }
        }

        // POST user
        [HttpPost]
        public async Task<ActionResult> RegisterUser([FromBody] NewUser value)
        {
            try
            {
                var userWithUsername = await _authorizationDbContext.Users.FirstOrDefaultAsync(user => user.Username == value.Username);

                if (userWithUsername is not null)
                {
                    return Conflict($"User with '{value.Username}' username already exist. (Id = {userWithUsername.Id})");
                }

                var userWithEmail = await _authorizationDbContext.Users.FirstOrDefaultAsync(user => user.Email == value.Email);
                if (userWithEmail is not null)
                {
                    return Conflict($"User with '{value.Email}' email already exist. (Id = {userWithEmail.Id})");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't get data from DB");
            }

            var userRole = await _authorizationDbContext.Roles.FirstOrDefaultAsync(role => role.Name == "User");
            if (userRole is not null)
            {
                value.Role = userRole;
            }

            try
            {
                await _authorizationDbContext.Users.AddAsync(value.ConverToDb());
                await _authorizationDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't add data to DB");
            }

            return Ok();
        }

        // PUT user/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserModel value)
        {
            DbUser? existingUser;
            try
            {
                value.Id = id;
                existingUser = await _authorizationDbContext.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't get data from DB");
            }

            if (existingUser is null)
            {
                return NotFound($"Can't get user by [{id}] id");
            }

            if (!IsRequestedUserAutorizedOrAdmin(existingUser))
            {
                return Forbid();
            }

            if (existingUser is null)
            {
                return NotFound();
            }

            if (value.FirstName is not null)
            {
                existingUser.FirstName = value.FirstName;
            }

            if (value.LastName is not null)
            {
                existingUser.LastName = value.LastName;
            }

            if (value.Email is not null)
            {
                existingUser.Email = value.Email;
            }

            if (value.Phone is not null)
            {
                existingUser.Phone = value.Phone;
            }

            if (value.Password is not null)
            {
                existingUser.PasswordHash = PasswordHasher.GetHash(value.Password);
            }

            try
            {
                _authorizationDbContext.Users.Update(existingUser);
                await _authorizationDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't update data to DB");
            }


            return Ok();
        }

        // DELETE user/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            DbUser? user;
            try
            {
                user = await _authorizationDbContext.Users.FindAsync(id);

                if (user == null)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't get data from DB");
            }

            if (!IsRequestedUserAutorizedOrAdmin(user))
            {
                return Forbid();
            }

            try
            {
                _authorizationDbContext.Users.Remove(user);
                await _authorizationDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't remove data from DB");
            }

            return Ok();
        }

        private bool IsRequestedUserAutorizedOrAdmin(DbUser user)
        {
            if (User.Identity?.Name is null)
            {
                return false;
            }

            return user.Username.Equals(User.Identity.Name, StringComparison.InvariantCultureIgnoreCase)
                || User.IsInRole(Constants.AdminRoleName);
        }
    }
}
