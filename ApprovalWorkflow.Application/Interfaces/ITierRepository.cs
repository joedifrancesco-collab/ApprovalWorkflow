using ApprovalWorkflow.Domain;

namespace ApprovalWorkflow.Application.Interfaces;

public interface ITierRepository
{
    Task<IEnumerable<Tier>> GetAllAsync();
    Task<bool> AnyAsync();
    Task CreateAsync(Tier tier);
}
