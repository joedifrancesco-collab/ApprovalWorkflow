using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using ApprovalWorkflow.Infrastructure.Data;
using Dapper;

namespace ApprovalWorkflow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public UserRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByUserNumberAsync(string userNumber)
    {
        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<User>(
            "SELECT Id, FirstName, LastName, UserNumber, Email, PasswordHash, IsActive, MustChangePassword, CreatedAt FROM Users WHERE UserNumber = @UserNumber",
            new { UserNumber = userNumber });
    }

    public async Task<bool> AnyAsync()
    {
        using var conn = _connectionFactory.Create();
        var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Users");
        return count > 0;
    }

    public async Task CreateAsync(User user)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(
            "INSERT INTO Users (FirstName, LastName, UserNumber, Email, PasswordHash) VALUES (@FirstName, @LastName, @UserNumber, @Email, @PasswordHash)",
            user);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<User>(
            "SELECT Id, FirstName, LastName, UserNumber, Email, PasswordHash, IsActive, MustChangePassword, CreatedAt FROM Users WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var conn = _connectionFactory.Create();
        return await conn.QueryAsync<User>(
            "SELECT Id, FirstName, LastName, UserNumber, Email, IsActive, MustChangePassword, CreatedAt FROM Users ORDER BY LastName, FirstName");
    }

    public async Task<bool> UserNumberExistsAsync(string userNumber, int? excludeId = null)
    {
        using var conn = _connectionFactory.Create();
        if (excludeId.HasValue)
            return await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Users WHERE UserNumber = @UserNumber AND Id <> @ExcludeId",
                new { UserNumber = userNumber, ExcludeId = excludeId.Value }) > 0;
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Users WHERE UserNumber = @UserNumber",
            new { UserNumber = userNumber }) > 0;
    }

    public async Task UpdateProfileAsync(User user)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(
            "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, UserNumber = @UserNumber, Email = @Email WHERE Id = @Id",
            user);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleNameAsync(string roleName)
    {
        using var conn = _connectionFactory.Create();
        return await conn.QueryAsync<User>(@"
            SELECT u.Id, u.FirstName, u.LastName, u.UserNumber, u.Email, u.IsActive, u.MustChangePassword, u.CreatedAt
            FROM Users u
            JOIN UserXRoles uxr ON u.Id = uxr.UserID
            JOIN UserRoles ur   ON uxr.RoleID = ur.RoleID
            WHERE ur.RoleName = @RoleName",
            new { RoleName = roleName });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var conn = _connectionFactory.Create();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT Id, FirstName, LastName, UserNumber, Email, PasswordHash, IsActive, MustChangePassword, CreatedAt FROM Users WHERE Email = @Email",
            new { Email = email });
    }

    public async Task UpdatePasswordHashAsync(int userId, string passwordHash)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(
            "UPDATE Users SET PasswordHash = @PasswordHash WHERE Id = @UserId",
            new { UserId = userId, PasswordHash = passwordHash });
    }

    public async Task UpdateEmailAsync(int userId, string email)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(
            "UPDATE Users SET Email = @Email WHERE Id = @UserId",
            new { UserId = userId, Email = email });
    }

    public async Task SetIsActiveAsync(int userId, bool isActive)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(
            "UPDATE Users SET IsActive = @IsActive WHERE Id = @UserId",
            new { UserId = userId, IsActive = isActive });
    }

    public async Task SetMustChangePasswordAsync(int userId, bool mustChange)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(
            "UPDATE Users SET MustChangePassword = @MustChange WHERE Id = @UserId",
            new { UserId = userId, MustChange = mustChange });
    }
}
