using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using ApprovalWorkflow.Infrastructure.Data;
using Dapper;

namespace ApprovalWorkflow.Infrastructure.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public PasswordResetTokenRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(PasswordResetToken token)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(
            "INSERT INTO PasswordResetTokens (UserId, Token, ExpiresAt, IsUsed) VALUES (@UserId, @Token, @ExpiresAt, @IsUsed)",
            token);
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token)
    {
        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<PasswordResetToken>(
            "SELECT TokenId, UserId, Token, ExpiresAt, IsUsed FROM PasswordResetTokens WHERE Token = @Token",
            new { Token = token });
    }

    public async Task MarkUsedAsync(int tokenId)
    {
        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(
            "UPDATE PasswordResetTokens SET IsUsed = 1 WHERE TokenId = @TokenId",
            new { TokenId = tokenId });
    }
}
