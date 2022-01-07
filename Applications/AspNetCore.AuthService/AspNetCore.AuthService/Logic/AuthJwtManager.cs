using AspNetCore.AuthService.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;

namespace AspNetCore.AuthService.Logic
{
    public sealed class AuthJwtManager
    {
        private readonly RsaSecurityKey _privateKey;

        public AuthJwtManager(IOptions<TokenOptions> authOptions)
        {
            var authOptionsValue = authOptions.Value;

            var privateKeyBytes = Convert.FromBase64String(authOptionsValue.PrivateKey);
            var privateCrypto = new RSACryptoServiceProvider(1024);
            privateCrypto.ImportRSAPrivateKey(privateKeyBytes, out var countPrivate);
            _privateKey = new RsaSecurityKey(privateCrypto);
        }

        internal SecurityKey GetPrivateSecurityKey()
        {
            return _privateKey;
        }
    }
}
