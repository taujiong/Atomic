using Atomic.Identity.Api.Models;
using AutoMapper;

namespace Atomic.Identity.Api.Dtos;

public class IdentityMapperProfile : Profile
{
    public IdentityMapperProfile()
    {
        CreateMap<AppUser, IdentityUserOutputDto>();
    }
}