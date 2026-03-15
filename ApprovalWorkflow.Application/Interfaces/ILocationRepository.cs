using ApprovalWorkflow.Domain;

namespace ApprovalWorkflow.Application.Interfaces;

public interface ILocationRepository
{
    Task<IEnumerable<Location>> GetAllAsync();
    Task<Location?> GetByIdAsync(int id);
    Task<bool> AnyAsync();
    Task CreateAsync(Location location);
    Task UpdateAsync(Location location);
    Task DeleteAsync(int id);
    Task<bool> IsInUseAsync(int id);
}
