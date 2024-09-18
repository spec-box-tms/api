using AutoMapper;
using SpecBox.Domain.Model.Users;
using SpecBox.WebApi.Model.Auth;

namespace SpecBox.WebApi.Model;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, UserResponse>();
    }
}
