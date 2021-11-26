using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Atomic.IdentityServer.Api.Models;

public class IdentityServerMapperProfile : Profile
{
    public IdentityServerMapperProfile()
    {
        CreateMap<Client, ClientListOutputDto>();
        CreateMap<Client, ClientOutputDto>().ForMember(des => des.PostLogoutRedirectUris,
            opt => opt.MapFrom(src => src.PostLogoutRedirectUris.Select(x => x.PostLogoutRedirectUri))
        ).ForMember(des => des.RedirectUris,
            opt => opt.MapFrom(src => src.RedirectUris.Select(x => x.RedirectUri))
        ).ForMember(des => des.AllowedGrantTypes,
            opt => opt.MapFrom(src => src.AllowedGrantTypes.Select(x => x.GrantType))
        ).ForMember(des => des.AllowedScopes,
            opt => opt.MapFrom(src => src.AllowedScopes.Select(x => x.Scope))
        ).ForMember(des => des.ClientSecrets,
            opt => opt.MapFrom(src => src.ClientSecrets.Select(x => new ClientSecretMaskedDto
            {
                Id = x.Id,
                Description = x.Description,
                Created = x.Created,
                MaskedValue = x.Value.Substring(x.Value.Length - 9, 8).PadLeft(14, '*'),
            }))
        );
    }
}