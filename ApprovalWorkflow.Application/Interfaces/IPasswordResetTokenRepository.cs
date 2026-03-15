using ApprovalWorkflow.Domain;

namespace ApprovalWorkflow.Application.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task CreateAsync(PasswordResetToken token);
    Task<PasswordResetToken?> GetByTokenAsync(string token);
    Task MarkUsedAsync(int tokenId);
}
