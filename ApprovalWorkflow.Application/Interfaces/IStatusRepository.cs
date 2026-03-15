using ApprovalWorkflow.Domain;

namespace ApprovalWorkflow.Application.Interfaces;

public interface IStatusRepository
{
    Task<IEnumerable<RequestStatus>> GetAllAsync();
    Task<RequestStatus?> GetByIdAsync(int id);
    Task<RequestStatus?> GetByNameAsync(string name);
    Task<bool> AnyAsync();
    Task CreateAsync(RequestStatus status);
    Task UpdateAsync(RequestStatus status);
    Task DeleteAsync(int id);
    Task<bool> IsInUseAsync(int id);
}
