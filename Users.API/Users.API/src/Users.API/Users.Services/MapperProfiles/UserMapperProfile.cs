using AutoMapper;
using User.Models.Commands;
using User.Models.Responses;

namespace User.Services.MapperProfiles
{
    public class UserMapperProfile: Profile
    {
        public UserMapperProfile()
        {
            CreateMap<Domain.Entities.User, UserResponse>();
            CreateMap<UserUpdateCommand,Domain.Entities.User>();
            CreateMap<Domain.Entities.User, UserCommand>();
            CreateMap<UserCommand, Domain.Entities.User>();
        }
    }
};

