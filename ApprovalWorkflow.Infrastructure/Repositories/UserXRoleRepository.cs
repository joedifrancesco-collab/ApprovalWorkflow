using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using ApprovalWorkflow.Infrastructure.Data;
using Dapper;

namespace ApprovalWorkflow.Infrastructure.Repositories;

public class UserXRoleRepository : IUserXRoleRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public UserXRoleRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<UserXRole?> GetByUserIdAsync(int userId)
    {
        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<UserXRole>(
            "SELECT UXRID, UserID, RoleID FROM UserXRoles WHERE UserID = @UserID",
            new { UserID = userId });
    }

    public async Task CreateAsync(UserXRole userXRole)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(
            "INSERT INTO UserXRoles (UserID, RoleID) VALUES (@UserID, @RoleID)",
            userXRole);
    }

    public async Task UpsertAsync(int userId, int roleId)
    {
        using var conn = _connectionFactory.Create();
        var existing = await conn.QuerySingleOrDefaultAsync<UserXRole>(
            "SELECT UXRID, UserID, RoleID FROM UserXRoles WHERE UserID = @UserID",
            new { UserID = userId });
        if (existing is null)
            await conn.ExecuteAsync(
                "INSERT INTO UserXRoles (UserID, RoleID) VALUES (@UserID, @RoleID)",
                new { UserID = userId, RoleID = roleId });
        else
            await conn.ExecuteAsync(
                "UPDATE UserXRoles SET RoleID = @RoleID WHERE UserID = @UserID",
                new { UserID = userId, RoleID = roleId });
    }
}
