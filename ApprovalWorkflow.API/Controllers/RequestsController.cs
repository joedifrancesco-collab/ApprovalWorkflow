using ApprovalWorkflow.Application.DTOs;
using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ApprovalWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly ICardRequestRepository    _requestRepository;
    private readonly IStatusRepository         _statusRepository;
    private readonly IRequestAuditLogRepository _auditRepository;
    private readonly IUserRepository           _userRepository;

    public RequestsController(
        ICardRequestRepository     requestRepository,
        IStatusRepository          statusRepository,
        IRequestAuditLogRepository auditRepository,
        IUserRepository            userRepository)
    {
        _requestRepository = requestRepository;
        _statusRepository  = statusRepository;
        _auditRepository   = auditRepository;
        _userRepository    = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var requests = await _requestRepository.GetAllAsync();
        return Ok(requests);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var detail = await _requestRepository.GetByIdAsync(id);
        if (detail is null) return NotFound();
        detail.AuditLog = (await _auditRepository.GetByRequestIdAsync(id)).ToList();
        return Ok(detail);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRequestDto dto)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(dto.CompanyName))   errors.Add("Company Name is required.");
        if (string.IsNullOrWhiteSpace(dto.FirstName))     errors.Add("First Name is required.");
        if (string.IsNullOrWhiteSpace(dto.LastName))      errors.Add("Last Name is required.");
        if (string.IsNullOrWhiteSpace(dto.Address1))      errors.Add("Address is required.");
        if (string.IsNullOrWhiteSpace(dto.City))          errors.Add("City is required.");
        if (string.IsNullOrWhiteSpace(dto.StateProvince)) errors.Add("State/Province is required.");
        if (string.IsNullOrWhiteSpace(dto.Zip))           errors.Add("Zip is required.");
        if (dto.TierID <= 0)                              errors.Add("Tier is required.");
        if (string.IsNullOrWhiteSpace(dto.ExpirationDate)) errors.Add("Expiration Date is required.");

        if (!DateTime.TryParse(dto.ExpirationDate, out var expDate))
            errors.Add("Expiration Date is invalid.");

        if (errors.Count > 0)
            return BadRequest(new { errors });

        var status = await _statusRepository.GetByNameAsync("Awaiting Approval");
        if (status is null)
            return StatusCode(500, "Status configuration error.");

        var request = new CardRequest
        {
            CompanyName       = dto.CompanyName,
            JobTitle          = dto.JobTitle,
            FirstName         = dto.FirstName,
            LastName          = dto.LastName,
            Address1          = dto.Address1,
            Address2          = dto.Address2,
            City              = dto.City,
            StateProvince     = dto.StateProvince,
            Zip               = dto.Zip,
            Country           = string.IsNullOrWhiteSpace(dto.Country) ? "UNITED STATES" : dto.Country,
            TierID            = dto.TierID,
            ExpirationDate    = expDate,
            CardStatus        = "Not Printed",
            Location          = dto.Location,
            Manager           = dto.Manager,
            ApproverUserID    = dto.ApproverUserID,
            Notes             = dto.Notes,
            StatusID          = status.StatusID,
            SubmittedByUserID = dto.SubmittedByUserID
        };

        var requestId = await _requestRepository.CreateAsync(request);

        var user     = await _userRepository.GetByIdAsync(dto.SubmittedByUserID);
        var userName = user is not null ? $"{user.FirstName} {user.LastName}" : "Unknown";

        await _auditRepository.CreateAsync(new RequestAuditLog
        {
            RequestID = requestId,
            UserID    = dto.SubmittedByUserID,
            Action    = $"{DateTime.Now:yyyy-MM-dd}: User {userName} submitted a new request.",
            LoggedAt  = DateTime.Now
        });

        return Ok(new { requestId });
    }

    // POST /api/requests/{id}/approve
    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] RequestActionDto req)
    {
        var detail = await _requestRepository.GetByIdAsync(id);
        if (detail is null) return NotFound();

        var status = await _statusRepository.GetByNameAsync("Approved");
        if (status is null) return StatusCode(500, "Status configuration error.");

        await _requestRepository.UpdateStatusAsync(id, status.StatusID);

        var actingUser = await _userRepository.GetByIdAsync(req.UserId);
        var actorName  = actingUser is not null ? $"{actingUser.FirstName} {actingUser.LastName}" : "Unknown";

        await _auditRepository.CreateAsync(new RequestAuditLog
        {
            RequestID = id,
            UserID    = req.UserId,
            Action    = $"{DateTime.Now:yyyy-MM-dd}: Request approved by {actorName}.",
            LoggedAt  = DateTime.Now
        });

        return Ok(new { submittedBy = detail.SubmittedBy });
    }

    // POST /api/requests/{id}/reject
    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] RequestActionDto req)
    {
        var detail = await _requestRepository.GetByIdAsync(id);
        if (detail is null) return NotFound();

        var status = await _statusRepository.GetByNameAsync("Rejected");
        if (status is null) return StatusCode(500, "Status configuration error.");

        await _requestRepository.UpdateStatusAsync(id, status.StatusID);

        var actingUser = await _userRepository.GetByIdAsync(req.UserId);
        var actorName  = actingUser is not null ? $"{actingUser.FirstName} {actingUser.LastName}" : "Unknown";

        await _auditRepository.CreateAsync(new RequestAuditLog
        {
            RequestID = id,
            UserID    = req.UserId,
            Action    = $"{DateTime.Now:yyyy-MM-dd}: Request rejected by {actorName}.",
            LoggedAt  = DateTime.Now
        });

        return Ok(new { submittedBy = detail.SubmittedBy });
    }
}
