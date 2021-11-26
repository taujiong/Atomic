using System.Linq.Dynamic.Core;
using AppService.Dtos;
using Atomic.AppService.Dtos;
using Atomic.AppService.Services;
using Atomic.AspNetCore.Mvc;
using Atomic.AspNetCore.Users;
using Atomic.ExceptionHandling;
using Atomic.IdentityServer.Api.Data;
using Atomic.IdentityServer.Api.Extensions;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atomic.IdentityServer.Api.Controllers;

public class ClientController : AtomicControllerBase,
    ICrudAppService<string, ClientOutputDto, ClientListOutputDto, ClientCreateUpdateDto, ClientCreateUpdateDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public ClientController(
        AppDbContext dbContext,
        IMapper mapper,
        ICurrentUser currentUser
    )
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    [HttpGet("{id}")]
    public async Task<ClientOutputDto> GetById(string id)
    {
        var client = await _dbContext.Clients
            .WithDetails()
            .FirstOrDefaultAsync(c => id.Equals(c.ClientId));
        if (client == null) throw new EntityNotFoundException(typeof(Client), id);

        return _mapper.Map<Client, ClientOutputDto>(client);
    }

    [HttpGet]
    public async Task<PagedResultDto<ClientListOutputDto>> GetListByQuery(QueryRequestDto input)
    {
        var totalCount = await _dbContext.Clients.CountAsync();
        var clients = await _dbContext.Clients
            .WhereIf(
                !string.IsNullOrEmpty(input.Filter),
                c => c.ClientName.Contains(input.Filter!))
            .OrderBy(input.Sort ?? "ClientName ASC")
            .PageBy(input.SkipCount, input.MaxResultCount)
            .ToListAsync();

        var clientDtos = _mapper.Map<List<Client>, List<ClientListOutputDto>>(clients);
        return new PagedResultDto<ClientListOutputDto>(totalCount, clientDtos);
    }

    [HttpPost]
    public async Task<ClientOutputDto> Create(ClientCreateUpdateDto input)
    {
        var client = new Client
        {
            ClientId = Guid.NewGuid().ToString("N"),
            AllowAccessTokensViaBrowser = input.AllowAccessTokensViaBrowser,
            RedirectUris = new List<ClientRedirectUri>(),
            PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>(),
            AllowedScopes = new List<ClientScope>(),
            AllowedGrantTypes = new List<ClientGrantType>(),
            Properties = new List<ClientProperty>
            {
                new()
                {
                    Key = "Creator",
                    Value = _currentUser.UserName ?? "boot",
                },
            },
        };

        client.MapFrom(input);
        client.GenerateNewSecret();
        var savedClient = await _dbContext.Clients.AddAsync(client);
        await _dbContext.SaveChangesAsync(true);

        return _mapper.Map<Client, ClientOutputDto>(savedClient.Entity);
    }

    [HttpPut("{id}")]
    public async Task<ClientOutputDto> UpdateById(string id, ClientCreateUpdateDto input)
    {
        var client = await _dbContext.Clients
            .WithDetails()
            .FirstOrDefaultAsync(c => id.Equals(c.ClientId));
        if (client == null) throw new EntityNotFoundException(typeof(Client), id);

        client.MapFrom(input);
        var savedClient = _dbContext.Clients.Update(client);
        await _dbContext.SaveChangesAsync(true);

        return _mapper.Map<Client, ClientOutputDto>(savedClient.Entity);
    }

    [HttpDelete("{id}")]
    public async Task DeleteById(string id)
    {
        var client = await _dbContext.Clients.FirstOrDefaultAsync(c => id.Equals(c.ClientId));
        if (client == null) throw new EntityNotFoundException(typeof(Client), id);

        _dbContext.Clients.Remove(client);
        await _dbContext.SaveChangesAsync(true);
    }

    [HttpDelete("{id}/secrets")]
    public async Task DeleteSecret(string id, int secretId)
    {
        var client = await _dbContext.Clients
            .Include(c => c.ClientSecrets)
            .FirstOrDefaultAsync(c => id.Equals(c.ClientId));
        if (client == null) throw new EntityNotFoundException(typeof(Client), id);

        var secret = client.ClientSecrets.Find(s => s.Id == secretId);
        if (secret == null) throw new EntityNotFoundException(typeof(ClientSecret), secretId);

        client.ClientSecrets.Remove(secret);
        await _dbContext.SaveChangesAsync();
    }

    [HttpPost("{id}/secrets")]
    public async Task<ClientSecretDto> GenerateNewSecret(string id)
    {
        var client = await _dbContext.Clients
            .Include(c => c.ClientSecrets)
            .FirstOrDefaultAsync(c => id.Equals(c.ClientId));
        if (client == null) throw new EntityNotFoundException(typeof(Client), id);

        var secretDto = client.GenerateNewSecret();
        await _dbContext.SaveChangesAsync();

        return secretDto;
    }
}