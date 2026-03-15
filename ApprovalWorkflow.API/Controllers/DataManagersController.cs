using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ApprovalWorkflow.API.Controllers;

[ApiController]
[Route("api/data-managers")]
public class DataManagersController : ControllerBase
{
    private readonly ILocationRepository  _locationRepository;
    private readonly IStatusRepository    _statusRepository;
    private readonly IUserRoleRepository  _userRoleRepository;

    public DataManagersController(
        ILocationRepository  locationRepository,
        IStatusRepository    statusRepository,
        IUserRoleRepository  userRoleRepository)
    {
        _locationRepository = locationRepository;
        _statusRepository   = statusRepository;
        _userRoleRepository = userRoleRepository;
    }

    // ── LOCATIONS ───────────────────────────────────────────────────────────

    [HttpGet("locations")]
    public async Task<IActionResult> GetLocations()
        => Ok(await _locationRepository.GetAllAsync());

    [HttpPost("locations")]
    public async Task<IActionResult> CreateLocation([FromBody] NameDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "Name is required." });

        await _locationRepository.CreateAsync(new Location { LocationName = dto.Name.Trim() });
        return Ok();
    }

    [HttpPut("locations/{id:int}")]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] NameDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "Name is required." });

        var existing = await _locationRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.LocationName = dto.Name.Trim();
        await _locationRepository.UpdateAsync(existing);
        return Ok();
    }

    [HttpDelete("locations/{id:int}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var existing = await _locationRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (await _locationRepository.IsInUseAsync(id))
            return Conflict(new { message = "This location is referenced by one or more requests and cannot be deleted." });

        await _locationRepository.DeleteAsync(id);
        return Ok();
    }

    // ── STATUSES ────────────────────────────────────────────────────────────

    [HttpGet("statuses")]
    public async Task<IActionResult> GetStatuses()
        => Ok(await _statusRepository.GetAllAsync());

    [HttpPost("statuses")]
    public async Task<IActionResult> CreateStatus([FromBody] NameDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "Name is required." });

        await _statusRepository.CreateAsync(new RequestStatus { StatusName = dto.Name.Trim() });
        return Ok();
    }

    [HttpPut("statuses/{id:int}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] NameDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "Name is required." });

        var existing = await _statusRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.StatusName = dto.Name.Trim();
        await _statusRepository.UpdateAsync(existing);
        return Ok();
    }

    [HttpDelete("statuses/{id:int}")]
    public async Task<IActionResult> DeleteStatus(int id)
    {
        if (await _statusRepository.IsInUseAsync(id))
            return Conflict(new { message = "This status is in use by one or more requests and cannot be deleted." });

        await _statusRepository.DeleteAsync(id);
        return Ok();
    }

    // ── ROLES ────────────────────────────────────────────────────────────────

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
        => Ok(await _userRoleRepository.GetAllAsync());

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] RoleDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RoleName))
            return BadRequest(new { message = "Role name is required." });

        await _userRoleRepository.CreateAsync(new UserRole
        {
            RoleName        = dto.RoleName.Trim(),
            RoleDescription = dto.RoleDescription?.Trim() ?? string.Empty,
            SortOrder       = dto.SortOrder
        });
        return Ok();
    }

    [HttpPut("roles/{id:int}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RoleName))
            return BadRequest(new { message = "Role name is required." });

        var existing = await _userRoleRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.RoleName        = dto.RoleName.Trim();
        existing.RoleDescription = dto.RoleDescription?.Trim() ?? string.Empty;
        existing.SortOrder       = dto.SortOrder;
        await _userRoleRepository.UpdateAsync(existing);
        return Ok();
    }

    [HttpDelete("roles/{id:int}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var existing = await _userRoleRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();

        if (await _userRoleRepository.IsInUseAsync(id))
            return Conflict(new { message = "This role is assigned to one or more users and cannot be deleted." });

        await _userRoleRepository.DeleteAsync(id);
        return Ok();
    }
}

public record NameDto(string Name);
public record RoleDto(string RoleName, string? RoleDescription, int SortOrder);
