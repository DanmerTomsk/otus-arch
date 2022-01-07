using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace AspNetCore.UserService.Helpers
{
    internal static class RsaKeyCreator
    {
        internal static RsaSecurityKey GetPublicSecurityKey(string publicRsaKey)
        {
            var publicKeyBytes = Convert.FromBase64String(publicRsaKey);
            var publicCrypto = new RSACryptoServiceProvider(1024);
            publicCrypto.ImportRSAPublicKey(publicKeyBytes, out var countPublic);
            return new RsaSecurityKey(publicCrypto);
        }
    }
}
