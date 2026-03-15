using ApprovalWorkflow.Domain;

namespace ApprovalWorkflow.Application.Interfaces;

public interface IUserRoleRepository
{
    Task<IEnumerable<UserRole>> GetAllAsync();
    Task<UserRole?> GetByIdAsync(int roleId);
    Task<UserRole?> GetByNameAsync(string name);
    Task<bool> AnyAsync();
    Task CreateAsync(UserRole role);
    Task UpdateAsync(UserRole role);
    Task DeleteAsync(int roleId);
    Task<bool> IsInUseAsync(int roleId);
}
