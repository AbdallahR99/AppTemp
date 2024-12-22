using AppTemp.Core.Identity.Users.Dtos;

namespace AppTemp.Core.Identity.Users.Features.AssignUserRole;
public class AssignUserRoleCommand
{
    public List<UserRoleDetail> UserRoles { get; set; } = new();
}
