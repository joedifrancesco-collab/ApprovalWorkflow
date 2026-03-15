namespace ApprovalWorkflow.Domain;

public class CardRequest
{
    public int RequestID { get; set; }
    // Company
    public string CompanyName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    // Contact
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address1 { get; set; } = string.Empty;
    public string? Address2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string StateProvince { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string Country { get; set; } = "UNITED STATES";
    // Card
    public int TierID { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string CardStatus { get; set; } = "Not Printed";
    // Relationship
    public string? Location { get; set; }
    public string? Manager { get; set; }
    public int? ApproverUserID { get; set; }
    // Notes
    public string? Notes { get; set; }
    // Routing
    public int StatusID { get; set; }
    public int SubmittedByUserID { get; set; }
    public DateTime CreatedAt { get; set; }
}
