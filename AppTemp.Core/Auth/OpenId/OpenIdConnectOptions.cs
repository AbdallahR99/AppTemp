using AppTemp.Core.Options;

namespace AppTemp.Core.Auth.OpenId
{
    public class OpenIdConnectOptions : IOptionsRoot
    {
        [Required(AllowEmptyStrings = false)]
        public string? Authority { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false)]
        public string? Audience { get; set; } = string.Empty;
        public string? MetadataAddress { get; set; } = string.Empty;
        public string? ClientId { get; set; } = string.Empty;
        public string? ClientSecret { get; set; } = string.Empty;
        public string? CallbackPath { get; set; } = string.Empty;
        public string? CallbackUrl { get; set; } = string.Empty;
        public string? TokenEndpoint { get; set; } = string.Empty;
        public string? UserInfoEndpoint { get; set; } = string.Empty;
        public string CookieName { get; set; } = "oidc";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return this.ValidateKeys(ClientId, ClientSecret, CallbackPath, CallbackUrl);


        }
    }
}
