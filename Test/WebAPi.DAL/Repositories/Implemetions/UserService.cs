using Microsoft.EntityFrameworkCore;
using WebAPi.DAL.DTO;
using WebAPi.DAL.Entity;
using WebAPi.DAL.Enum;
using WebAPi.DAL.Repositories.Interfaces;

namespace WebAPi.DAL.Repositories.Implemetions;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize, string sortBy, string sortOrder, string nameFilter, int? ageFilter, string emailFilter)
    {
        var query = _dbContext.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            query = query.Where(u => u.Name.Contains(nameFilter));
        }

        if (ageFilter.HasValue)
        {
            query = query.Where(u => u.Age == ageFilter.Value);
        }

        if (!string.IsNullOrWhiteSpace(emailFilter))
        {
            query = query.Where(u => u.Email.Contains(emailFilter));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(u => EF.Property<object>(u, sortBy));
            }
            else
            {
                query = query.OrderByDescending(u => EF.Property<object>(u, sortBy));
            }
        }

        var skip = (page - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);

        return await query.ToListAsync();
    }

    public async Task<UserWithRolesDto> GetUserByIdAsync(int id)
    {
        var userDto = await _dbContext.Users
            .Where(u => u.Id == id)
            .Select(u => new UserWithRolesDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Age = u.Age,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .FirstOrDefaultAsync();

        return userDto;
    }

    public async Task<User> CreateUserAsync(UserDto userDto)
    {
        if (string.IsNullOrWhiteSpace(userDto.Name) || string.IsNullOrWhiteSpace(userDto.Email))
        {
            throw new ArgumentException("Insufficient data to create a user.");
        }

        if (await _dbContext.Users.AnyAsync(u => u.Email == userDto.Email))
        {
            throw new InvalidOperationException("User with such Email already exists.");
        }

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var userRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RoleName.User.ToString());
            if (userRole == null)
            {
                userRole = new Role { Name = RoleName.User.ToString() };
                _dbContext.Roles.Add(userRole);
                await _dbContext.SaveChangesAsync();
            }

            var user = new User
            {
                Name = userDto.Name,
                Age = userDto.Age,
                Email = userDto.Email,
                UserRoles = new List<UserRole> { new UserRole { Role = userRole } }
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            transaction.Commit();

            return user;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw ex;
        }
    }

    public async Task<User> UpdateUserAsync(int id, UserDto user)
    {
        var existingUser = await _dbContext.Users.FindAsync(id);
        if (existingUser == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        if (await _dbContext.Users.AnyAsync(u => u.Email == user.Email && u.Id != id))
        {
            throw new InvalidOperationException("A user with this Email already exists.");
        }

        existingUser.Name = user.Name;
        existingUser.Age = user.Age;
        existingUser.Email = user.Email;

        await _dbContext.SaveChangesAsync();

        return existingUser;
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _dbContext.Users.FindAsync(id);

        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {id} not found.");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

}