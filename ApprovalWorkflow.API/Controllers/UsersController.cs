using ApprovalWorkflow.Application.DTOs;
using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;

namespace ApprovalWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository      _userRepository;
    private readonly IUserRoleRepository  _userRoleRepository;
    private readonly IUserXRoleRepository _userXRoleRepository;

    public UsersController(
        IUserRepository      userRepository,
        IUserRoleRepository  userRoleRepository,
        IUserXRoleRepository userXRoleRepository)
    {
        _userRepository      = userRepository;
        _userRoleRepository  = userRoleRepository;
        _userXRoleRepository = userXRoleRepository;
    }

    // GET /api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userRepository.GetAllAsync();
        var roles = (await _userRoleRepository.GetAllAsync()).ToDictionary(r => r.RoleID);

        var result = new List<UserListDto>();
        foreach (var u in users)
        {
            var assignment = await _userXRoleRepository.GetByUserIdAsync(u.Id);
            var roleName   = string.Empty;
            var roleId     = 0;
            if (assignment is not null && roles.TryGetValue(assignment.RoleID, out var role))
            {
                roleName = role.RoleName;
                roleId   = role.RoleID;
            }
            result.Add(new UserListDto
            {
                Id                 = u.Id,
                FirstName          = u.FirstName,
                LastName           = u.LastName,
                UserNumber         = u.UserNumber,
                Email              = u.Email,
                IsActive           = u.IsActive,
                MustChangePassword = u.MustChangePassword,
                RoleName           = roleName,
                RoleID             = roleId,
                CreatedAt          = u.CreatedAt
            });
        }
        return Ok(result);
    }

    // GET /api/users/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var u = await _userRepository.GetByIdAsync(id);
        if (u is null) return NotFound();

        var assignment = await _userXRoleRepository.GetByUserIdAsync(u.Id);
        var roleName   = string.Empty;
        var roleId     = 0;
        if (assignment is not null)
        {
            var role = await _userRoleRepository.GetByIdAsync(assignment.RoleID);
            if (role is not null) { roleName = role.RoleName; roleId = role.RoleID; }
        }

        return Ok(new UserListDto
        {
            Id                 = u.Id,
            FirstName          = u.FirstName,
            LastName           = u.LastName,
            UserNumber         = u.UserNumber,
            Email              = u.Email,
            IsActive           = u.IsActive,
            MustChangePassword = u.MustChangePassword,
            RoleName           = roleName,
            RoleID             = roleId,
            CreatedAt          = u.CreatedAt
        });
    }

    // POST /api/users
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(req.FirstName))  errors.Add("First name is required.");
        if (string.IsNullOrWhiteSpace(req.LastName))   errors.Add("Last name is required.");
        if (string.IsNullOrWhiteSpace(req.UserNumber)) errors.Add("User number is required.");
        if (string.IsNullOrWhiteSpace(req.Password))   errors.Add("Password is required.");
        if (req.Password.Length < 8)                   errors.Add("Password must be at least 8 characters.");
        if (req.RoleID <= 0)                            errors.Add("A role must be selected.");

        if (!errors.Any() && await _userRepository.UserNumberExistsAsync(req.UserNumber))
            errors.Add($"User number '{req.UserNumber}' is already taken.");

        if (errors.Any()) return BadRequest(new { errors });

        var user = new User
        {
            FirstName          = req.FirstName.Trim(),
            LastName           = req.LastName.Trim(),
            UserNumber         = req.UserNumber.Trim(),
            Email              = req.Email?.Trim() ?? string.Empty,
            PasswordHash       = BC.HashPassword(req.Password),
            IsActive           = true,
            MustChangePassword = false
        };
        await _userRepository.CreateAsync(user);

        // Fetch newly created user to get the Id
        var created = await _userRepository.GetByUserNumberAsync(user.UserNumber);
        if (created is not null)
            await _userXRoleRepository.UpsertAsync(created.Id, req.RoleID);

        return Ok(new { message = "User created." });
    }

    // PUT /api/users/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest req)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing is null) return NotFound();

        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(req.FirstName))  errors.Add("First name is required.");
        if (string.IsNullOrWhiteSpace(req.LastName))   errors.Add("Last name is required.");
        if (string.IsNullOrWhiteSpace(req.UserNumber)) errors.Add("User number is required.");
        if (req.RoleID <= 0)                            errors.Add("A role must be selected.");

        if (!errors.Any() && await _userRepository.UserNumberExistsAsync(req.UserNumber.Trim(), id))
            errors.Add($"User number '{req.UserNumber}' is already taken.");

        if (errors.Any()) return BadRequest(new { errors });

        existing.FirstName  = req.FirstName.Trim();
        existing.LastName   = req.LastName.Trim();
        existing.UserNumber = req.UserNumber.Trim();
        existing.Email      = req.Email?.Trim() ?? string.Empty;
        await _userRepository.UpdateProfileAsync(existing);
        await _userXRoleRepository.UpsertAsync(id, req.RoleID);

        return Ok(new { message = "User updated." });
    }

    // POST /api/users/{id}/set-password
    [HttpPost("{id:int}/set-password")]
    public async Task<IActionResult> SetPassword(int id, [FromBody] AdminSetPasswordRequest req)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();

        if (string.IsNullOrWhiteSpace(req.NewPassword) || req.NewPassword.Length < 8)
            return BadRequest(new { message = "Password must be at least 8 characters." });

        await _userRepository.UpdatePasswordHashAsync(id, BC.HashPassword(req.NewPassword));
        await _userRepository.SetMustChangePasswordAsync(id, req.MustChangePassword);

        return Ok(new { message = "Password updated." });
    }

    // POST /api/users/{id}/force-password-change
    [HttpPost("{id:int}/force-password-change")]
    public async Task<IActionResult> ForcePasswordChange(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();
        await _userRepository.SetMustChangePasswordAsync(id, true);
        return Ok(new { message = "User will be prompted to change password at next login." });
    }

    // POST /api/users/{id}/deactivate
    [HttpPost("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();
        await _userRepository.SetIsActiveAsync(id, false);
        return Ok(new { message = "User deactivated." });
    }

    // POST /api/users/{id}/activate
    [HttpPost("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return NotFound();
        await _userRepository.SetIsActiveAsync(id, true);
        return Ok(new { message = "User activated." });
    }
}
