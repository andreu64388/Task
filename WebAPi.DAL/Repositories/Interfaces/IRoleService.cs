using WebAPi.DAL.DTO;
using WebAPi.DAL.Entity;

namespace WebAPi.DAL.Repositories.Interfaces;

public interface IRoleService
{
    Task<Role> AddRoleToUserAsync(RoleDto roleDto);

    Task<IEnumerable<Role>> GetRolesAsync();
}