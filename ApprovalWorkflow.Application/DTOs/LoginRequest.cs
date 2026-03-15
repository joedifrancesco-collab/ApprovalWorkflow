namespace ApprovalWorkflow.Application.DTOs;

public class LoginRequest
{
    public string UserNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
