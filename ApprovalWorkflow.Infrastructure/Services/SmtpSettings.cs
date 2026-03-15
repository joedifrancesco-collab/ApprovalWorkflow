namespace ApprovalWorkflow.Infrastructure.Services;

public class SmtpSettings
{
    public string Host            { get; set; } = string.Empty;
    public int    Port            { get; set; } = 587;
    public string UserName        { get; set; } = string.Empty;
    public string Password        { get; set; } = string.Empty;
    public bool   EnableSsl       { get; set; } = true;
    public string FromAddress     { get; set; } = "noreply@example.com";
    public string FromDisplayName { get; set; } = "Approval Workflow";
}
