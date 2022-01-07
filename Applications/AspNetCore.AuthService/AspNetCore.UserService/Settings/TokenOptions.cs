using System.ComponentModel.DataAnnotations;

namespace AspNetCore.UserService.Settings
{
    internal sealed class TokenOptions
    {
        public const string SettingsSectionName = "TokenOptions";

        [Required]
        internal string PublicKey { get; init; } = null!;

        [Required]
        internal string Issuer { get; init; } = null!;
    }
}
