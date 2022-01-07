using AspNetCore.TestApp.Models.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AspNetCore.TestApp.Utilities
{
    public sealed class AuthJwtManager
    {
        private readonly AuthOptions _authOptions;

        public AuthJwtManager(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions.Value;
        }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authOptions.JwtKey));
        }
    }
}
