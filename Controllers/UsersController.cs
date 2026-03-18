using Microsoft.AspNetCore.Mvc;
using UserManagementApi.DTOs;
using UserManagementApi.Services;

namespace UserManagementApi.Controllers;


[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService users, ILogger<UsersController> logger)
    {
        _users  = users;
        _logger = logger;
    }

    // ─────────────────────────────────────────────────────────────
    // GET  /api/users
    // ─────────────────────────────────────────────────────────────
    /// <summary>Returns all users, with optional role / active filters.</summary>
    /// <param name="role">Filter by role: Admin, User, or Moderator.</param>
    /// <param name="isActive">Filter by active status.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] string? role, [FromQuery] bool? isActive)
    {
        _logger.LogInformation("Fetching users – role={Role}, isActive={IsActive}", role, isActive);
        var result = _users.GetAll(role, isActive);
        return Ok(result);
    }

    // ─────────────────────────────────────────────────────────────
    // GET  /api/users/{id}
    // ─────────────────────────────────────────────────────────────

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var user = _users.GetById(id);
        if (user is null)
        {
            _logger.LogWarning("User {Id} not found.", id);
            return NotFound(new { error = $"User {id} not found." });
        }
        return Ok(user);
    }

    // ─────────────────────────────────────────────────────────────
    // POST /api/users
    // ─────────────────────────────────────────────────────────────
  
    [HttpPost]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (_users.EmailExists(dto.Email))
        {
            _logger.LogWarning("Create failed – email {Email} already exists.", dto.Email);
            return Conflict(new { error = $"Email '{dto.Email}' is already in use." });
        }

        var created = _users.Create(dto);
        _logger.LogInformation("Created user {Id} ({Email}).", created.Id, created.Email);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // ─────────────────────────────────────────────────────────────
    // PUT  /api/users/{id}
    // ─────────────────────────────────────────────────────────────
   
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Update(int id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (dto.Email is not null && _users.EmailExists(dto.Email, excludeId: id))
        {
            _logger.LogWarning("Update failed – email {Email} already taken.", dto.Email);
            return Conflict(new { error = $"Email '{dto.Email}' is already in use by another user." });
        }

        var updated = _users.Update(id, dto);
        if (updated is null)
        {
            _logger.LogWarning("Update failed – user {Id} not found.", id);
            return NotFound(new { error = $"User {id} not found." });
        }

        _logger.LogInformation("Updated user {Id}.", id);
        return Ok(updated);
    }

    // ─────────────────────────────────────────────────────────────
    // DELETE /api/users/{id}
    // ─────────────────────────────────────────────────────────────

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        if (!_users.Delete(id))
        {
            _logger.LogWarning("Delete failed – user {Id} not found.", id);
            return NotFound(new { error = $"User {id} not found." });
        }

        _logger.LogInformation("Deleted user {Id}.", id);
        return NoContent();
    }
}
