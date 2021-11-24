using System.Text;
using Atomic.AspNetCore.Mvc;
using Atomic.ExceptionHandling;
using Atomic.Identity.Api.Extensions;
using Atomic.Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.OpenApi.Extensions;

namespace Atomic.Identity.Api.Controllers;

public class PasswordController : AtomicControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PasswordController> _logger;
    private readonly UserManager<AppUser> _userManager;

    public PasswordController(
        UserManager<AppUser> userManager,
        ILogger<PasswordController> logger,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPut("change-password")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task ChangePassword(ChangePasswordDto input)
    {
        var user = await _userManager.FindByIdAsync(input.Id);
        if (user == null) throw new EntityNotFoundException(typeof(AppUser), input.Id!);

        var changePasswordResult =
            await _userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
        changePasswordResult.CheckErrors();
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task SendResetPasswordLink(RequireResetPasswordDto input)
    {
        var userIdentifier = input.UserIdentifier;
        var user = await _userManager.FindByNameAsync(userIdentifier)
                   ?? await _userManager.FindByEmailAsync(userIdentifier)
                   ?? await _userManager.FindByIdAsync(userIdentifier)
                   ?? throw new EntityNotFoundException(typeof(AppUser), input.UserIdentifier!, "userIdentifier");

        var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        resetPasswordToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken));
        var baseUrl = _configuration["OAuthServer:IssuerUrl"];
        var resetUrl = $"{baseUrl}/Account/ResetPassword?token={resetPasswordToken}&userId={user.Id}";
        switch (input.Method)
        {
            case ContactMethod.Phone:
                // TODO: implement send token with phone
                _logger.LogInformation("Send by {Method}: {Url}",
                    input.Method.GetDisplayName().ToLowerInvariant(), resetUrl);
                break;
            case ContactMethod.Email:
            default:
                // TODO: implement send token with email
                _logger.LogInformation("Send by {Method}: {Url}",
                    input.Method.GetDisplayName().ToLowerInvariant(), resetUrl);
                break;
        }
    }

    [HttpPut("reset-password")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task ResetPassword(ResetPasswordDto input)
    {
        var user = await _userManager.FindByIdAsync(input.UserId);
        if (user == null) throw new EntityNotFoundException(typeof(AppUser), input.UserId!);

        var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(input.Token!));
        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, input.Password);
        resetPasswordResult.CheckErrors();
    }
}