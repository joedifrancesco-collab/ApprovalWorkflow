using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using ApprovalWorkflow.Infrastructure.Data;
using Dapper;

namespace ApprovalWorkflow.Infrastructure.Repositories;

public class StatusRepository : IStatusRepository
{
    private readonly DbConnectionFactory _factory;

    public StatusRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<RequestStatus>> GetAllAsync()
    {
        using var conn = _factory.Create();
        return await conn.QueryAsync<RequestStatus>("SELECT StatusID, StatusName FROM Status ORDER BY StatusID");
    }

    public async Task<RequestStatus?> GetByIdAsync(int id)
    {
        using var conn = _factory.Create();
        return await conn.QuerySingleOrDefaultAsync<RequestStatus>(
            "SELECT StatusID, StatusName FROM Status WHERE StatusID = @id",
            new { id });
    }

    public async Task<RequestStatus?> GetByNameAsync(string name)
    {
        using var conn = _factory.Create();
        return await conn.QuerySingleOrDefaultAsync<RequestStatus>(
            "SELECT StatusID, StatusName FROM Status WHERE StatusName = @Name",
            new { Name = name });
    }

    public async Task<bool> AnyAsync()
    {
        using var conn = _factory.Create();
        return await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Status") > 0;
    }

    public async Task CreateAsync(RequestStatus status)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync("INSERT INTO Status (StatusName) VALUES (@StatusName)", status);
    }

    public async Task UpdateAsync(RequestStatus status)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            "UPDATE Status SET StatusName = @StatusName WHERE StatusID = @StatusID", status);
    }

    public async Task<bool> IsInUseAsync(int id)
    {
        using var conn = _factory.Create();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM CardRequests WHERE StatusID = @id", new { id }) > 0;
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync("DELETE FROM Status WHERE StatusID = @id", new { id });
    }
}
