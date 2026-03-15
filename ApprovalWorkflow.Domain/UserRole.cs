namespace ApprovalWorkflow.Domain;

public class UserRole
{
    public int RoleID { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleDescription { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
