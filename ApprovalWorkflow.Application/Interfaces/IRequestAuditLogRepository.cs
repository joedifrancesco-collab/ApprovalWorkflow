using ApprovalWorkflow.Application.DTOs;
using ApprovalWorkflow.Domain;

namespace ApprovalWorkflow.Application.Interfaces;

public interface IRequestAuditLogRepository
{
    Task CreateAsync(RequestAuditLog log);
    Task<IEnumerable<AuditLogEntryDto>> GetByRequestIdAsync(int requestId);
}
