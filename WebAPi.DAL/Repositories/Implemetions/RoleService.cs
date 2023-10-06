using Microsoft.EntityFrameworkCore;
using WebAPi.DAL.DTO;
using WebAPi.DAL.Entity;
using WebAPi.DAL.Repositories.Interfaces;

namespace WebAPi.DAL.Repositories.Implemetions;

public class RoleService : IRoleService
{
    private readonly AppDbContext _dbContext;

    public RoleService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role> AddRoleToUserAsync(RoleDto roleDto)
    {
        var user = await _dbContext.Users.FindAsync(roleDto.UserId);
        if (user == null)
        {
            throw new InvalidOperationException("Пользователь не найден.");
        }

        if (user.UserRoles == null)
        {
            user.UserRoles = new List<UserRole>();
        }

        var existingUserRole = user.UserRoles.FirstOrDefault(ur => ur.Role.Name == roleDto.RoleName);

        if (existingUserRole == null)
        {
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleDto.RoleName);

            if (role == null)
            {
                throw new InvalidOperationException("Роль не найдена в базе данных.");
            }

            var userRoleExists = await _dbContext.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);

            if (!userRoleExists)
            {
                user.UserRoles.Add(new UserRole
                {
                    Role = role
                });

                await _dbContext.SaveChangesAsync();
                return role;
            }
            else
            {
                throw new InvalidOperationException("Роль уже существует для данного пользователя.");
            }
        }
        else
        {
            throw new InvalidOperationException("Роль уже существует для данного пользователя.");
        }
    }

    public async Task<IEnumerable<Role>> GetRolesAsync()
    {
        return await _dbContext.Roles.ToListAsync();
    }
}