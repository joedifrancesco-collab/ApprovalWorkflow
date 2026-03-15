using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using ApprovalWorkflow.Infrastructure.Data;
using Dapper;

namespace ApprovalWorkflow.Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public UserRoleRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<UserRole>> GetAllAsync()
    {
        using var conn = _connectionFactory.Create();
        return await conn.QueryAsync<UserRole>(
            "SELECT RoleID, RoleName, RoleDescription, SortOrder FROM UserRoles ORDER BY SortOrder");
    }

    public async Task<UserRole?> GetByIdAsync(int roleId)
    {
        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<UserRole>(
            "SELECT RoleID, RoleName, RoleDescription, SortOrder FROM UserRoles WHERE RoleID = @RoleID",
            new { RoleID = roleId });
    }

    public async Task<bool> AnyAsync()
    {
        using var conn = _connectionFactory.Create();
        var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM UserRoles");
        return count > 0;
    }

    public async Task CreateAsync(UserRole role)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(
            "INSERT INTO UserRoles (RoleName, RoleDescription, SortOrder) VALUES (@RoleName, @RoleDescription, @SortOrder)",
            role);
    }

    public async Task UpdateAsync(UserRole role)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(
            "UPDATE UserRoles SET RoleName = @RoleName, RoleDescription = @RoleDescription WHERE RoleID = @RoleID",
            role);
    }

    public async Task<UserRole?> GetByNameAsync(string name)
    {
        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<UserRole>(
            "SELECT RoleID, RoleName, RoleDescription, SortOrder FROM UserRoles WHERE RoleName = @Name",
            new { Name = name });
    }

    public async Task<bool> IsInUseAsync(int roleId)
    {
        using var conn = _connectionFactory.Create();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM UserXRole WHERE RoleID = @roleId", new { roleId }) > 0;
    }

    public async Task DeleteAsync(int roleId)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync("DELETE FROM UserRoles WHERE RoleID = @roleId", new { roleId });
    }
}
