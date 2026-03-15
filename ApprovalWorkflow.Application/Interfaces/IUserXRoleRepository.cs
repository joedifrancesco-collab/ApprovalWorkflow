using ApprovalWorkflow.Domain;

namespace ApprovalWorkflow.Application.Interfaces;

public interface IUserXRoleRepository
{
    Task<UserXRole?> GetByUserIdAsync(int userId);
    Task CreateAsync(UserXRole userXRole);
    Task UpsertAsync(int userId, int roleId);
}
