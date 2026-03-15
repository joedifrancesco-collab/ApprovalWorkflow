namespace ApprovalWorkflow.Application.DTOs;

public class RequestSummaryDto
{
    public int RequestID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string TierName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
