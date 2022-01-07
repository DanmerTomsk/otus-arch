using AspNetCore.Common.Helpers;
using AspNetCore.Common.Models;
using AspNetCore.UserService.Models;

namespace AspNetCore.UserService.Helpers
{
    internal static class UserConverter
    {
        internal static DbUser ConverToDb(this NewUser newUser)
        {
            return new DbUser
            {
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                PasswordHash = PasswordHasher.GetHash(newUser.Password),
                Phone = newUser.Phone,
                Role = newUser.Role,
                RoleId = newUser.RoleId,
                Username = newUser.Username,
            };
        }


    }
}
