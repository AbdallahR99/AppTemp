using System.ComponentModel;
using FluentValidation;


namespace AppTemp.Core.Identity.Tokens.Features.Generate;
public record TokenGenerationCommand(
    [property: DefaultValue("admin@admin.com")] string Email,
    [property: DefaultValue("DD@19375")] string Password);

public class GenerateTokenValidator : AbstractValidator<TokenGenerationCommand>
{
    public GenerateTokenValidator()
    {
        RuleFor(p => p.Email).Cascade(CascadeMode.Stop).NotEmpty().EmailAddress();

        RuleFor(p => p.Password).Cascade(CascadeMode.Stop).NotEmpty();
    }
}
