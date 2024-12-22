using FluentValidation;

namespace AppTemp.Core.Identity.Roles.Features.CreateOrUpdateRole;

public class CreateOrUpdateRoleValidator : AbstractValidator<CreateOrUpdateRoleCommand>
{
    public CreateOrUpdateRoleValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Role name is required.");
    }
}
