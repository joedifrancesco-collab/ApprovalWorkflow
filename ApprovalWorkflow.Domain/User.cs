namespace ApprovalWorkflow.Domain;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserNumber { get; set; } = string.Empty;
    public string Email              { get; set; } = string.Empty;
    public string PasswordHash       { get; set; } = string.Empty;
    public bool   IsActive           { get; set; } = true;
    public bool   MustChangePassword { get; set; } = false;
    public DateTime CreatedAt { get; set; }
}
