using AutoMapper;
using EduPlanManager.Models.DTOs.Subject;
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
            CreateMap<Subject, SubjectDetailDTO>()
            .ForMember(dest => dest.AcademicTermYear, opt => opt.MapFrom(src => src.AcademicTerm.Year))
            .ForMember(dest => dest.AcademicTermSemester, opt => opt.MapFrom(src => src.AcademicTerm.Semester))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.FullName));
            CreateMap<Subject, SubjectDTO>().ReverseMap();
        }
    }
}
