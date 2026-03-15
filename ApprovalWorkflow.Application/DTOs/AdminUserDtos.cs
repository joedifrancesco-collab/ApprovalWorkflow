namespace ApprovalWorkflow.Application.DTOs;

public class CreateUserRequest
{
    public string FirstName  { get; set; } = string.Empty;
    public string LastName   { get; set; } = string.Empty;
    public string UserNumber { get; set; } = string.Empty;
    public string Email      { get; set; } = string.Empty;
    public string Password   { get; set; } = string.Empty;
    public int    RoleID     { get; set; }
}

public class UpdateUserRequest
{
    public string FirstName  { get; set; } = string.Empty;
    public string LastName   { get; set; } = string.Empty;
    public string UserNumber { get; set; } = string.Empty;
    public string Email      { get; set; } = string.Empty;
    public int    RoleID     { get; set; }
}

public class AdminSetPasswordRequest
{
    public string NewPassword        { get; set; } = string.Empty;
    public bool   MustChangePassword { get; set; } = false;
}
