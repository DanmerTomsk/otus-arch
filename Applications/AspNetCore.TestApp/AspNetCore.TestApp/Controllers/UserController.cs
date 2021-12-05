using AspNetCore.TestApp.Db;
using AspNetCore.TestApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspNetCore.TestApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserDbContext _userDbContext;

        public UserController(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext; 
        }

        // GET: user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            try
            {
                var users = await _userDbContext.Users.ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't get data from DB", type: Constants.DbConnectionExceptionTypeName);
            }
        }

        // GET user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            try
            {
                var user = await _userDbContext.Users.FindAsync(id);

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
        public async Task<ActionResult> AddUser([FromBody] User value)
        {
            try
            { 
                var isUsernameExist = _userDbContext.Users.Any(user => user.Username == value.Username);
 
                if (isUsernameExist)
                {
                    return Conflict($"User with '{value.Username}' username already exist. (Id = {value.Id})");
                }

                var isEmailExist = _userDbContext.Users.Any(user => user.Email == value.Email);
                if (isEmailExist)
                {
                    return Conflict($"User with '{value.Email}' email already exist. (Id = {value.Id})");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't get data from DB");
            }

            try 
            { 
                await _userDbContext.Users.AddAsync(value);
                await _userDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't add data to DB");
            }

            return Ok();
        }

        // PUT user/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserModel value)
        {
            User existingUser;
            try
            {
                value.Id = id;
                existingUser = await _userDbContext.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't get data from DB");
            }

            if (existingUser == null)
            {
                return NotFound();
            }

            if (value.FirstName != null)
            {
                existingUser.FirstName = value.FirstName;
            }

            if (value.LastName != null)
            {
                existingUser.LastName = value.LastName;
            }

            if (value.Email != null)
            {
                existingUser.Email = value.Email;
            }

            if (value.Phone != null)
            {
                existingUser.Phone = value.Phone;
            }

            try
            {
                _userDbContext.Users.Update(existingUser);
                await _userDbContext.SaveChangesAsync();                
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't update data to DB");
            }


            return Ok();
        }

        // DELETE user/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            User user;
            try
            {
                user = await _userDbContext.Users.FindAsync(id);

                if (user == null)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't get data from DB");
            }

            try
            {
                _userDbContext.Users.Remove(user);
                await _userDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, title: "Can't remove data from DB");
            }

            return Ok();
        }
    }
}
