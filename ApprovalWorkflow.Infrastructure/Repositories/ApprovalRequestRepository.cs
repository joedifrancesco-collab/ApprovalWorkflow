using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using ApprovalWorkflow.Infrastructure.Data;
using Dapper;

namespace ApprovalWorkflow.Infrastructure.Repositories;

public class ApprovalRequestRepository : IApprovalRequestRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public ApprovalRequestRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<ApprovalRequest>> GetAllAsync()
    {
        using var conn = _connectionFactory.Create();
        return await conn.QueryAsync<ApprovalRequest>(
            "SELECT Id, Title, Status, CreatedAt FROM ApprovalRequests ORDER BY CreatedAt DESC");
    }

    public async Task<ApprovalRequest?> GetByIdAsync(int id)
    {
        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<ApprovalRequest>(
            "SELECT Id, Title, Status, CreatedAt FROM ApprovalRequests WHERE Id = @Id",
            new { Id = id });
    }
}
