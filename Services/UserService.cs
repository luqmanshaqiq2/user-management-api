using UserManagementApi.DTOs;
using UserManagementApi.Models;

namespace UserManagementApi.Services;

public interface IUserService
{
    IEnumerable<UserResponseDto> GetAll(string? role, bool? isActive);
    UserResponseDto? GetById(int id);
    UserResponseDto Create(CreateUserDto dto);
    UserResponseDto? Update(int id, UpdateUserDto dto);
    bool Delete(int id);
    bool EmailExists(string email, int? excludeId = null);
}

public class UserService : IUserService
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public UserService()
    {
        // Seed some initial data
        _users.AddRange(new[]
        {
            new User { Id = _nextId++, FirstName = "Alice",   LastName = "Johnson", Email = "alice@example.com",   Role = "Admin",     IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-30) },
            new User { Id = _nextId++, FirstName = "Bob",     LastName = "Smith",   Email = "bob@example.com",     Role = "User",      IsActive = true,  CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new User { Id = _nextId++, FirstName = "Carol",   LastName = "White",   Email = "carol@example.com",   Role = "Moderator", IsActive = false, CreatedAt = DateTime.UtcNow.AddDays(-10) },
        });
    }

    public IEnumerable<UserResponseDto> GetAll(string? role, bool? isActive)
    {
        var query = _users.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(role))
            query = query.Where(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase));

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        return query.Select(MapToDto);
    }

    public UserResponseDto? GetById(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return user is null ? null : MapToDto(user);
    }

    public UserResponseDto Create(CreateUserDto dto)
    {
        var user = new User
        {
            Id        = _nextId++,
            FirstName = dto.FirstName.Trim(),
            LastName  = dto.LastName.Trim(),
            Email     = dto.Email.Trim().ToLowerInvariant(),
            Role      = dto.Role,
            IsActive  = true,
            CreatedAt = DateTime.UtcNow
        };
        _users.Add(user);
        return MapToDto(user);
    }

    public UserResponseDto? Update(int id, UpdateUserDto dto)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user is null) return null;

        if (dto.FirstName is not null) user.FirstName = dto.FirstName.Trim();
        if (dto.LastName  is not null) user.LastName  = dto.LastName.Trim();
        if (dto.Email     is not null) user.Email     = dto.Email.Trim().ToLowerInvariant();
        if (dto.Role      is not null) user.Role      = dto.Role;
        if (dto.IsActive  is not null) user.IsActive  = dto.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;
        return MapToDto(user);
    }

    public bool Delete(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user is null) return false;
        _users.Remove(user);
        return true;
    }

    public bool EmailExists(string email, int? excludeId = null)
        => _users.Any(u => u.Email.Equals(email.Trim().ToLowerInvariant(), StringComparison.Ordinal)
                        && (excludeId is null || u.Id != excludeId));

    private static UserResponseDto MapToDto(User u) => new()
    {
        Id        = u.Id,
        FirstName = u.FirstName,
        LastName  = u.LastName,
        Email     = u.Email,
        Role      = u.Role,
        IsActive  = u.IsActive,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    };
}
