using System.Linq.Dynamic.Core;
using AppService.Dtos;
using Atomic.AppService.Dtos;
using Atomic.AppService.Services;
using Atomic.AspNetCore.Mvc;
using Atomic.Identity.Api.Dtos;
using Atomic.Identity.Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atomic.Identity.Api.Controllers;

public class UserController : AtomicControllerBase,
    ICrudAppService<string, IdentityUserOutputDto, IdentityUserCreateDto, IdentityUserUpdateDto>
{
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public UserController(UserManager<AppUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<IdentityUserOutputDto> GetById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) throw new Exception("User not found");

        return _mapper.Map<AppUser, IdentityUserOutputDto>(user);
    }

    [HttpGet]
    public async Task<PagedResultDto<IdentityUserOutputDto>> GetListByQuery(QueryRequestDto input)
    {
        var totalCount = await _userManager.Users.CountAsync();
        var users = await _userManager.Users
            .WhereIf(
                !string.IsNullOrEmpty(input.Filter),
                user => user.UserName.Contains(input.Filter!) || user.Email.Contains(input.Filter!))
            .PageBy(input.SkipCount, input.MaxResultCount)
            .OrderBy(input.Sort ?? "UserName ASC")
            .ToListAsync();

        var userDtos = _mapper.Map<List<AppUser>, List<IdentityUserOutputDto>>(users);
        return new PagedResultDto<IdentityUserOutputDto>(totalCount, userDtos);
    }

    [HttpPost]
    public async Task<IdentityUserOutputDto> Create(IdentityUserCreateDto input)
    {
        var user = new AppUser(input.UserName!)
        {
            Email = input.Email,
            PhoneNumber = input.PhoneNumber,
        };
        var createResult = await _userManager.CreateAsync(user, input.Password);
        if (createResult.Succeeded)
        {
            return _mapper.Map<AppUser, IdentityUserOutputDto>(user);
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
    public async Task<IdentityUserOutputDto> UpdateById(string id, IdentityUserUpdateDto input)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) throw new Exception("User not found");

        user.Email = input.Email;
        user.UserName = input.UserName;
        user.PhoneNumber = input.PhoneNumber;
        var updateResult = await _userManager.UpdateAsync(user);
        if (updateResult.Succeeded) return _mapper.Map<AppUser, IdentityUserOutputDto>(user);

        throw new Exception(updateResult.Errors.First().Description);
    }
}