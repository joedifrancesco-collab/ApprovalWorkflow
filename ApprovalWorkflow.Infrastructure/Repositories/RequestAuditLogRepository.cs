using ApprovalWorkflow.Application.DTOs;
using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using ApprovalWorkflow.Infrastructure.Data;
using Dapper;

namespace ApprovalWorkflow.Infrastructure.Repositories;

public class RequestAuditLogRepository : IRequestAuditLogRepository
{
    private readonly DbConnectionFactory _factory;

    public RequestAuditLogRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task CreateAsync(RequestAuditLog log)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            "INSERT INTO RequestAuditLog (RequestID, UserID, Action, LoggedAt) VALUES (@RequestID, @UserID, @Action, @LoggedAt)",
            log);
    }

    public async Task<IEnumerable<AuditLogEntryDto>> GetByRequestIdAsync(int requestId)
    {
        using var conn = _factory.Create();
        return await conn.QueryAsync<AuditLogEntryDto>(@"
            SELECT l.LogID,
                   u.FirstName + ' ' + u.LastName AS UserName,
                   l.Action,
                   l.LoggedAt
            FROM RequestAuditLog l
            JOIN Users u ON l.UserID = u.Id
            WHERE l.RequestID = @requestId
            ORDER BY l.LoggedAt",
            new { requestId });
    }
}
