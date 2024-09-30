using AutoMapper;
using SpecBox.Domain.Model.Users;
using SpecBox.WebApi.Model.Users;

namespace SpecBox.WebApi.Model;

public class UsersProfile : Profile
{
    public UsersProfile()
    {
        CreateMap<User, UserResponse>();
    }
}
