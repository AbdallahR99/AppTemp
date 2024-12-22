using AppTemp.Core.Options;
using System.ComponentModel.DataAnnotations;

namespace AppTemp.Core.Auth.Jwt;

public class JwtOptions : IOptionsRoot
{

    public string Key { get; set; } = string.Empty;
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int DurationInHours { get; set; }
    public int TokenExpirationInMinutes { get; set; }

    public int RefreshTokenExpirationInDays { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return this.ValidateKeys(Key, TokenExpirationInMinutes);
    }
}