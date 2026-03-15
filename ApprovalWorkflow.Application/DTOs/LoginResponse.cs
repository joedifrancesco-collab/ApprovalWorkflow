namespace ApprovalWorkflow.Application.DTOs;

public class LoginResponse
{
    public int    Id                 { get; set; }
    public string FirstName          { get; set; } = string.Empty;
    public string LastName           { get; set; } = string.Empty;
    public string UserNumber         { get; set; } = string.Empty;
    public int    RoleID             { get; set; }
    public string RoleName           { get; set; } = string.Empty;
    public bool   MustChangePassword { get; set; }
}
