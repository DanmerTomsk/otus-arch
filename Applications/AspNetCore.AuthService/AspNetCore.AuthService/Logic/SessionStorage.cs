using AspNetCore.AuthService.Settings;
using AspNetCore.Common.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace AspNetCore.AuthService.Logic
{
    public sealed class SessionStorage
    {
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        public readonly TimeSpan _tokenLifetime;

        private readonly AuthJwtManager _authJwtManager;
        private readonly TokenOptions _authOptions;

        public SessionStorage(AuthJwtManager authJwtManager, IOptions<TokenOptions> authOptions)
        {
            _authJwtManager = authJwtManager;
            _authOptions = authOptions.Value;
            _tokenLifetime = TimeSpan.FromMinutes(_authOptions.TokenLifetimeInMinutes);
        }

        internal Guid AddSession(DbUser user)
        {
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: _authOptions.Issuer,
                    notBefore: now,
                    claims: GetClaims(user),
                    expires: now.Add(TimeSpan.FromMinutes(_authOptions.TokenLifetimeInMinutes)),
                    signingCredentials: new SigningCredentials(_authJwtManager.GetPrivateSecurityKey(), SecurityAlgorithms.RsaSha512));
            
            Guid newSessionId;
            do
            {
                newSessionId = Guid.NewGuid();
            } while (_cache.TryGetValue(newSessionId, out _));

            _cache.Set(newSessionId, jwt, _tokenLifetime);
            return newSessionId;
        }

        internal bool TryGetSessionToken(Guid sessionId, out JwtSecurityToken? token)
        {
            return _cache.TryGetValue(sessionId, out token);
        }

        internal void RemoveSessionToken(Guid sessionId)
        {
            _cache.Remove(sessionId);
        }

        private static Claim[] GetClaims(DbUser user)
        {
            if (user.Role is null)
            {
                throw new InvalidOperationException($"Role for [{user.Username}] is empty");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims,
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity.Claims.ToArray();
        }
    }
}
