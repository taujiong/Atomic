using System.Linq.Dynamic.Core;
using AppService.Dtos;
using Atomic.AppService.Dtos;
using Atomic.AppService.Services;
using Atomic.AspNetCore.Mvc;
using Atomic.Identity.Api.Dtos;
using Atomic.Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atomic.Identity.Api.Controllers;

public class UserController : AtomicControllerBase,
    ICrudAppService<string, AppUser, IdentityUserCreateDto, IdentityUserUpdateDto>
{
    private readonly UserManager<AppUser> _userManager;

    public UserController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("{id}")]
    public async Task<AppUser> GetById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) throw new Exception("User not found");
        return user;
    }

    [HttpGet]
    public async Task<PagedResultDto<AppUser>> GetListByQuery(QueryRequestDto input)
    {
        var totalCount = await _userManager.Users.CountAsync();
        var users = await _userManager.Users
            .WhereIf(
                !string.IsNullOrEmpty(input.Filter),
                user => user.UserName.Contains(input.Filter!) || user.Email.Contains(input.Filter!))
            .PageBy(input.SkipCount, input.MaxResultCount)
            .OrderBy(input.Sort ?? "UserName ASC")
            .ToListAsync();

        return new PagedResultDto<AppUser>(totalCount, users);
    }

    [HttpPost]
    public async Task<AppUser> Create(IdentityUserCreateDto input)
    {
        var user = new AppUser(input.UserName!)
        {
            Email = input.Email,
            PhoneNumber = input.PhoneNumber,
        };
        var createResult = await _userManager.CreateAsync(user, "1q2w3E*");
        if (createResult.Succeeded)
        {
            return user;
        }

        throw new Exception(createResult.Errors.First().Description);
    }

    [HttpDelete("{id}")]
    public async Task DeleteById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) throw new Exception("user not fount");
        var deleteResult = await _userManager.DeleteAsync(user);
        if (deleteResult.Succeeded) return;
        throw new Exception(deleteResult.Errors.First().Description);
    }

    [HttpPut("{id}")]
    public async Task<AppUser> UpdateById(string id, IdentityUserUpdateDto input)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) throw new Exception("User not found");
        user.Email = input.Email;
        user.UserName = input.UserName;
        user.PhoneNumber = input.PhoneNumber;
        var updateResult = await _userManager.UpdateAsync(user);
        if (updateResult.Succeeded) return user;
        throw new Exception(updateResult.Errors.First().Description);
    }
}