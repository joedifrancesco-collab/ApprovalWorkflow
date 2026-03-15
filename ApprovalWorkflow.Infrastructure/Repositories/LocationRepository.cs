using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using ApprovalWorkflow.Infrastructure.Data;
using Dapper;

namespace ApprovalWorkflow.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly DbConnectionFactory _factory;

    public LocationRepository(DbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<Location>> GetAllAsync()
    {
        using var conn = _factory.Create();
        return await conn.QueryAsync<Location>(
            "SELECT LocationID, LocationName FROM Locations ORDER BY LocationName");
    }

    public async Task<bool> AnyAsync()
    {
        using var conn = _factory.Create();
        return await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Locations") > 0;
    }

    public async Task CreateAsync(Location location)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            "INSERT INTO Locations (LocationName) VALUES (@LocationName)", location);
    }

    public async Task<Location?> GetByIdAsync(int id)
    {
        using var conn = _factory.Create();
        return await conn.QuerySingleOrDefaultAsync<Location>(
            "SELECT LocationID, LocationName FROM Locations WHERE LocationID = @id", new { id });
    }

    public async Task UpdateAsync(Location location)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync(
            "UPDATE Locations SET LocationName = @LocationName WHERE LocationID = @LocationID", location);
    }

    public async Task<bool> IsInUseAsync(int id)
    {
        using var conn = _factory.Create();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM CardRequests WHERE Location = (SELECT LocationName FROM Locations WHERE LocationID = @id)",
            new { id }) > 0;
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = _factory.Create();
        await conn.ExecuteAsync("DELETE FROM Locations WHERE LocationID = @id", new { id });
    }
}
