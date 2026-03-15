using ApprovalWorkflow.Domain;

namespace ApprovalWorkflow.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUserNumberAsync(string userNumber);
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetUsersByRoleNameAsync(string roleName);
    Task<bool> AnyAsync();
    Task<bool> UserNumberExistsAsync(string userNumber, int? excludeId = null);
    Task CreateAsync(User user);
    Task UpdateProfileAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task UpdatePasswordHashAsync(int userId, string passwordHash);
    Task UpdateEmailAsync(int userId, string email);
    Task SetIsActiveAsync(int userId, bool isActive);
    Task SetMustChangePasswordAsync(int userId, bool mustChange);
}
