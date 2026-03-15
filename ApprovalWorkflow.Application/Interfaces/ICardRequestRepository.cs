using ApprovalWorkflow.Application.DTOs;
using ApprovalWorkflow.Domain;

namespace ApprovalWorkflow.Application.Interfaces;

public interface ICardRequestRepository
{
    Task<IEnumerable<RequestSummaryDto>> GetAllAsync();
    Task<RequestDetailDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CardRequest request);
    Task UpdateStatusAsync(int requestId, int statusId);
}
