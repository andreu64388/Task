using Microsoft.AspNetCore.Mvc;
using WebAPi.DAL.DTO;
using WebAPi.DAL.Repositories.Interfaces;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger, IUserService userService, IRoleService roleService)
    {
        _userService = userService;
        _roleService = roleService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "Id",
            [FromQuery] string sortOrder = "asc",
            [FromQuery] string nameFilter = null,
            [FromQuery] int? ageFilter = null,
            [FromQuery] string emailFilter = null)
    {
        try
        {
            _logger.LogInformation("GetUsers method called");
            var users = await _userService.GetUsersAsync(page, pageSize, sortBy, sortOrder, nameFilter, ageFilter, emailFilter);

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUsers method");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            _logger.LogInformation("GetUserById method called with ID {UserId}", id);

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserById method");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserDto user)
    {
        try
        {
            _logger.LogInformation("CreateUser method called");
            var createdUser = await _userService.CreateUserAsync(user);
            return Ok(createdUser);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error in CreateUser method");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error in CreateUser method");
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto user)
    {
        try
        {
            _logger.LogInformation("UpdateUser method called with ID {UserId}", id);
            var updatedUser = await _userService.UpdateUserAsync(id, user);
            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(updatedUser);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error in UpdateUser method");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error in UpdateUser method");
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }

    [HttpPost("/add-role")]
    public async Task<IActionResult> AddRoleToUser(RoleDto roleDto)
    {
        try
        {
            _logger.LogInformation("AddRoleToUser method called");
            await _roleService.AddRoleToUserAsync(roleDto);
            return Ok("Роль успешно добавлена.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error in UpdateUser method");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            _logger.LogInformation("GetAllRoles method called");
            var roles = await _roleService.GetRolesAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllRoles method");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}