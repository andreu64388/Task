using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPi.DAL.DTO;
using WebAPi.DAL.Entity;
using WebAPi.DAL.Repositories.Interfaces;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("User management")]
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
    [SwaggerOperation(
       Summary = "Get a list of users",
       Description = "This endpoint returns a list of all users.",
       Tags = new[] { "Users" }
   )]
    [SwaggerResponse(200, "List of users", typeof(IEnumerable<UserDto>))]
    [SwaggerResponse(500, "Internal server error")]
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
    [SwaggerOperation(
    Summary = "Get user by ID",
    Description = "This endpoint retrieves a user by their ID.",
    Tags = new[] { "Users" }
)]
    [SwaggerResponse(200, "User found", typeof(UserDto))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(500, "Internal server error")]
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
    [SwaggerOperation(
    Summary = "Create a new user",
    Description = "This endpoint creates a new user.",
    Tags = new[] { "Users" }
)]
    [SwaggerResponse(200, "User created", typeof(UserDto))]
    [SwaggerResponse(400, "Bad request")]
    [SwaggerResponse(409, "User already exists")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> CreateUser([FromBody] UserDto user)
    {
        try
        {
            _logger.LogInformation("CreateUser method called");
            var createdUser = await _userService.CreateUserAsync(user);
            _logger.LogInformation("User successfully created. UserID: {UserId}", createdUser.Id);

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
    [SwaggerOperation(
      Summary = "Update user by ID",
      Description = "This endpoint updates a user by their ID.",
      Tags = new[] { "Users" }
  )]
    [SwaggerResponse(200, "User updated", typeof(UserDto))]
    [SwaggerResponse(400, "Bad request")]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(409, "Conflict - User already exists with the same email")]
    [SwaggerResponse(500, "Internal server error")]
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
    [SwaggerOperation(
        Summary = "Delete user by ID",
        Description = "This endpoint deletes a user by their ID.",
        Tags = new[] { "Users" }
    )]
    [SwaggerResponse(204, "User deleted")]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            _logger.LogInformation("DeleteUser method called with ID {UserId}", id);
            await _userService.DeleteUserAsync(id);

            _logger.LogInformation("User with ID {UserId} has been deleted.", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error in DeleteUser method");
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("/add-role")]
    [SwaggerOperation(
    Summary = "Add a role to a user",
    Description = "This endpoint adds a role to a user.",
    Tags = new[] { "Users" }
)]
    [SwaggerResponse(200, "Role added successfully")]
    [SwaggerResponse(400, "Bad request")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> AddRoleToUser(RoleDto roleDto)
    {
        try
        {
            _logger.LogInformation("AddRoleToUser method called");
            await _roleService.AddRoleToUserAsync(roleDto);

            _logger.LogInformation("Role successfully added to user.");

            return Ok("Role successfully added.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error in UpdateUser method");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("roles")]
    [SwaggerOperation(
    Summary = "Get all roles",
    Description = "This endpoint retrieves a list of all roles.",
    Tags = new[] { "Users" }
)]
    [SwaggerResponse(200, "List of roles", typeof(IEnumerable<Role>))]
    [SwaggerResponse(500, "Internal server error")]
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