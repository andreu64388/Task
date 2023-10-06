using WebAPi.DAL.DTO;
using WebAPi.DAL.Entity;

namespace WebAPi.DAL.Repositories.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize, string sortBy, string sortOrder, string nameFilter, int? ageFilter, string emailFilter);

    Task<UserWithRolesDto> GetUserByIdAsync(int id);

    Task<User> CreateUserAsync(UserDto user);

    Task<User> UpdateUserAsync(int id, UserDto user);

    Task DeleteUserAsync(int id);
}