using AutoMapper;
using SpecBox.Domain.Model.Teams;
using SpecBox.WebApi.Model.Teams;

namespace SpecBox.WebApi.Model;

public class TeamProfile : Profile
{
    public TeamProfile()
    {
        CreateMap<Team, TeamResponse>();
        CreateMap<TeamUser, TeamUserResponse>();
    }
}
