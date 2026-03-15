using ApprovalWorkflow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApprovalWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupsController : ControllerBase
{
    private readonly ITierRepository      _tierRepository;
    private readonly IUserRepository      _userRepository;
    private readonly ILocationRepository  _locationRepository;
    private readonly IUserRoleRepository  _userRoleRepository;

    public LookupsController(
        ITierRepository     tierRepository,
        IUserRepository     userRepository,
        ILocationRepository locationRepository,
        IUserRoleRepository userRoleRepository)
    {
        _tierRepository     = tierRepository;
        _userRepository     = userRepository;
        _locationRepository = locationRepository;
        _userRoleRepository = userRoleRepository;
    }

    [HttpGet("tiers")]
    public async Task<IActionResult> GetTiers()
    {
        var tiers = await _tierRepository.GetAllAsync();
        return Ok(tiers);
    }

    [HttpGet("approvers")]
    public async Task<IActionResult> GetApprovers()
    {
        var approvers = await _userRepository.GetUsersByRoleNameAsync("Approver");
        return Ok(approvers.Select(u => new { u.Id, u.FirstName, u.LastName }));
    }

    [HttpGet("locations")]
    public async Task<IActionResult> GetLocations()
    {
        var locations = await _locationRepository.GetAllAsync();
        return Ok(locations);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _userRoleRepository.GetAllAsync();
        return Ok(roles.Select(r => new { id = r.RoleID, name = r.RoleName }));
    }
}
