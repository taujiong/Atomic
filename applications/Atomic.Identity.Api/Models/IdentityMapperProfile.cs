using AutoMapper;

namespace Atomic.Identity.Api.Models;

public class IdentityMapperProfile : Profile
{
    public IdentityMapperProfile()
    {
        CreateMap<AppUser, IdentityUserOutputDto>();
    }
}