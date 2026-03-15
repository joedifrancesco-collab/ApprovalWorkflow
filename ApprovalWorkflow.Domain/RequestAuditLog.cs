namespace ApprovalWorkflow.Domain;

public class RequestAuditLog
{
    public int LogID { get; set; }
    public int RequestID { get; set; }
    public int UserID { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime LoggedAt { get; set; }
}
