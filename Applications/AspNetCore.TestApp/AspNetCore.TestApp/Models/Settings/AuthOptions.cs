namespace AspNetCore.TestApp.Models.Settings
{
    public sealed class AuthOptions
    {
        public const string SettingsSectionName = "AuthorizationOptions";

        public AuthOptions(string jwtKey, string issuer)
        {
            JwtKey = jwtKey;
            Issuer = issuer;
        }

        public string JwtKey { get; }

        public string Issuer { get; }
    }
}
