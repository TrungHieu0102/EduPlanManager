using AutoMapper;
using EduPlanManager.Models.DTOs.User;
using EduPlanManager.Models.Entities;

namespace EduPlanManager.Mappings
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User,UpdateUserDto>().ReverseMap();
        }
    }
}
