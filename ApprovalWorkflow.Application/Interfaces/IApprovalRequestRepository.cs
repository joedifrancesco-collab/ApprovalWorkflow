using ApprovalWorkflow.Domain;

namespace ApprovalWorkflow.Application.Interfaces;

public interface IApprovalRequestRepository
{
    Task<IEnumerable<ApprovalRequest>> GetAllAsync();
    Task<ApprovalRequest?> GetByIdAsync(int id);
}
