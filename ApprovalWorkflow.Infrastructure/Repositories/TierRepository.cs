using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using ApprovalWorkflow.Infrastructure.Data;
using Dapper;

namespace ApprovalWorkflow.Infrastructure.Repositories;

public class TierRepository : ITierRepository
{
    private readonly DbConnectionFactory _factory;

    public TierRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Tier>> GetAllAsync()
    {
        using var conn = _factory.Create();
        return await conn.QueryAsync<Tier>("SELECT TierID, TierName FROM Tier ORDER BY TierID");
    }

    public async Task<bool> AnyAsync()
    {
        using var conn = _factory.Create();
        return await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Tier") > 0;
    }

    public async Task CreateAsync(Tier tier)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync("INSERT INTO Tier (TierName) VALUES (@TierName)", tier);
    }
}
