namespace ApprovalWorkflow.Application.DTOs;

public class CreateRequestDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address1 { get; set; } = string.Empty;
    public string? Address2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string StateProvince { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string Country { get; set; } = "UNITED STATES";
    public int TierID { get; set; }
    public string ExpirationDate { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Manager { get; set; }
    public int? ApproverUserID { get; set; }
    public string? Notes { get; set; }
    public int SubmittedByUserID { get; set; }
}
