using Atomic.AspNetCore.Mvc;
using Atomic.ExceptionHandling;
using Atomic.Identity.Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Atomic.Identity.Api.Controllers;

public class LoginController : AtomicControllerBase
{
    private readonly IMapper _mapper;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public LoginController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IMapper mapper
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    [HttpPost("password")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(501)] // TODO: remove it if two factor login is implemented
    public async Task<ActionResult<IdentityUserOutputDto>> LoginWithPassword(PasswordLoginDto input)
    {
        var user = await _userManager.FindByNameAsync(input.UserNameOrEmail)
                   ?? await _userManager.FindByEmailAsync(input.UserNameOrEmail)
                   ?? throw new EntityNotFoundException(typeof(AppUser), input.UserNameOrEmail!, "userNameOrEmail");

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, input.Password, true);
        signInResult.CheckErrors();

        if (signInResult.RequiresTwoFactor)
        {
            throw new NotImplementedException("This application has not supported two factor login yet");
        }

        return _mapper.Map<AppUser, IdentityUserOutputDto>(user);
    }

    [HttpPost("external")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IdentityUserOutputDto>> LoginWithExternal(ExternalLoginDto input)
    {
        var user = await _userManager.FindByLoginAsync(input.LoginProvider, input.ProviderKey)
                   ?? throw new EntityNotFoundException(typeof(AppUser), $"{input.LoginProvider} - {input.ProviderKey}",
                       "externalLoginInfo");

        if (!await _signInManager.CanSignInAsync(user))
        {
            throw new AuthException(AuthError.IsNotAllowed);
        }

        var userIsLockedOut = _userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user);
        if (userIsLockedOut)
        {
            throw new AuthException(AuthError.IsLockedOut);
        }

        return _mapper.Map<AppUser, IdentityUserOutputDto>(user);
    }
}