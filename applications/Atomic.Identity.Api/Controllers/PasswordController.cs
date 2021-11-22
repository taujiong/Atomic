using Atomic.AspNetCore.Mvc;
using Atomic.ExceptionHandling;
using Atomic.Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace Atomic.Identity.Api.Controllers;

public class PasswordController : AtomicControllerBase
{
    private readonly ILogger<PasswordController> _logger;
    private readonly UserManager<AppUser> _userManager;

    public PasswordController(
        UserManager<AppUser> userManager,
        ILogger<PasswordController> logger
    )
    {
        _userManager = userManager;
        _logger = logger;
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

    [HttpGet("reset-password")]
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
        // TODO: generate url with user id and token
        switch (input.Method)
        {
            case ContactMethod.Phone:
                // TODO: implement send token with phone
                _logger.LogInformation("Send by {Method} to user {UserId} with token {Token}",
                    input.Method.GetDisplayName().ToLowerInvariant(),
                    user.Id, resetPasswordToken);
                break;
            case ContactMethod.Email:
            default:
                // TODO: implement send token with email
                _logger.LogInformation("Send by {Method} to user {UserId} with token {Token}",
                    input.Method.GetDisplayName().ToLowerInvariant(),
                    user.Id, resetPasswordToken);
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

        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, input.Token, input.Password);
        resetPasswordResult.CheckErrors();
    }
}