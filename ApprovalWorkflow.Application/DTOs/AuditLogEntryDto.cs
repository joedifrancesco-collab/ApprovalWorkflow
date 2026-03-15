namespace ApprovalWorkflow.Application.DTOs;

public class AuditLogEntryDto
{
    public int LogID { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime LoggedAt { get; set; }
}
