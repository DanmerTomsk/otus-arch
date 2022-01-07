using System.ComponentModel.DataAnnotations;

namespace AspNetCore.AuthService.Settings
{
    public sealed class TokenOptions
    {
        public const string SettingsSectionName = "TokenOptions";

        [Required]
        internal string PrivateKey { get; init; } = null!;

        [Required]
        public string Issuer { get; init; } = null!;

        [Required]
        public int TokenLifetimeInMinutes { get; init; }
    }
}
