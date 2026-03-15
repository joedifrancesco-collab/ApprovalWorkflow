using ApprovalWorkflow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApprovalWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApprovalRequestsController : ControllerBase
{
    private readonly IApprovalRequestRepository _repository;

    public ApprovalRequestsController(IApprovalRequestRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var requests = await _repository.GetAllAsync();
        return Ok(requests);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var request = await _repository.GetByIdAsync(id);
        if (request is null)
            return NotFound();
        return Ok(request);
    }
}
