using ApprovalWorkflow.Application.DTOs;
using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;

namespace ApprovalWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository              _userRepository;
    private readonly IUserXRoleRepository         _userXRoleRepository;
    private readonly IUserRoleRepository          _userRoleRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IEmailService                _emailService;
    private readonly ILogger<AuthController>      _logger;

    public AuthController(
        IUserRepository              userRepository,
        IUserXRoleRepository         userXRoleRepository,
        IUserRoleRepository          userRoleRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IEmailService                emailService,
        ILogger<AuthController>      logger)
    {
        _userRepository              = userRepository;
        _userXRoleRepository         = userXRoleRepository;
        _userRoleRepository          = userRoleRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _emailService                = emailService;
        _logger                      = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserNumber) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("UserNumber and Password are required.");

        var user = await _userRepository.GetByUserNumberAsync(request.UserNumber);

        if (user is null || !BC.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid user number or password." });

        if (!user.IsActive)
            return Unauthorized(new { message = "This account has been deactivated. Please contact your administrator." });

        var assignment = await _userXRoleRepository.GetByUserIdAsync(user.Id);
        var roleName = string.Empty;
        var roleId   = 0;

        if (assignment is not null)
        {
            var role = await _userRoleRepository.GetByIdAsync(assignment.RoleID);
            if (role is not null)
            {
                roleName = role.RoleName;
                roleId   = role.RoleID;
            }
        }

        return Ok(new LoginResponse
        {
            Id                 = user.Id,
            FirstName          = user.FirstName,
            LastName           = user.LastName,
            UserNumber         = user.UserNumber,
            RoleID             = roleId,
            RoleName           = roleName,
            MustChangePassword = user.MustChangePassword
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        // Always return the same response to prevent user enumeration
        const string genericOk = "If that email address is on file, a reset link has been sent.";

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
            if (user is not null)
            {
                var resetToken = new PasswordResetToken
                {
                    UserId    = user.Id,
                    Token     = Guid.NewGuid().ToString(),
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IsUsed    = false
                };
                await _passwordResetTokenRepository.CreateAsync(resetToken);

                var baseUrl  = $"{Request.Scheme}://{Request.Host}";
                var resetUrl = $"{baseUrl}/?token={resetToken.Token}";

                try
                {
                    await _emailService.SendPasswordResetAsync(
                        user.Email,
                        $"{user.FirstName} {user.LastName}",
                        resetUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send password reset email to {Email}", request.Email);
                }
            }
        }

        return Ok(new { message = genericOk });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest(new { message = "Token and new password are required." });

        if (request.NewPassword.Length < 8)
            return BadRequest(new { message = "Password must be at least 8 characters." });

        var tokenRecord = await _passwordResetTokenRepository.GetByTokenAsync(request.Token.Trim());

        if (tokenRecord is null || tokenRecord.IsUsed || tokenRecord.ExpiresAt < DateTime.UtcNow)
            return BadRequest(new { message = "This reset link is invalid or has expired." });

        await _userRepository.UpdatePasswordHashAsync(tokenRecord.UserId, BC.HashPassword(request.NewPassword));
        await _passwordResetTokenRepository.MarkUsedAsync(tokenRecord.TokenId);
        await _userRepository.SetMustChangePasswordAsync(tokenRecord.UserId, false);

        return Ok(new { message = "Your password has been reset. You can now sign in." });
    }
}
