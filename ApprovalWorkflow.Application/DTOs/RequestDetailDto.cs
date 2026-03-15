namespace ApprovalWorkflow.Application.DTOs;

public class RequestDetailDto
{
    public int RequestID { get; set; }
    // Company
    public string CompanyName { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    // Contact
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address1 { get; set; } = string.Empty;
    public string? Address2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string StateProvince { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    // Card
    public string TierName { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public string CardStatus { get; set; } = string.Empty;
    // Relationship
    public string? Location { get; set; }
    public string? Manager { get; set; }
    public string? ApproverName { get; set; }
    // Notes
    public string? Notes { get; set; }
    // Routing
    public string StatusName { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    // Audit
    public List<AuditLogEntryDto> AuditLog { get; set; } = new();
}
