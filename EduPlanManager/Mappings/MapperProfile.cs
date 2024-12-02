using AutoMapper;
using EduPlanManager.Models.DTOs.AcademicTerm;
using EduPlanManager.Models.DTOs.Class;
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
            CreateMap<User, UpdateUserDto>().ReverseMap();
            CreateMap<User, UserInListDTO>()
          .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
          .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Classes.FirstOrDefault().ClassName));
            CreateMap<Subject, SubjectDetailDTO>()
            .ForMember(dest => dest.AcademicTermYear, opt => opt.MapFrom(src => src.AcademicTerm.Year))
            .ForMember(dest => dest.AcademicTermSemester, opt => opt.MapFrom(src => src.AcademicTerm.Semester))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.FullName));
            CreateMap<Subject, SubjectDTO>()
                 .ForMember(dest => dest.AcademicTermYear, opt => opt.MapFrom(src => src.AcademicTerm.Year.ToString()))
                .ForMember(dest => dest.AcademicTermSemester, opt => opt.MapFrom(src => src.AcademicTerm.Semester.ToString())).ReverseMap();
            CreateMap<Subject, SubjectUpdateDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.FullName))
                .ForMember(dest => dest.AcademicTermYear, opt => opt.MapFrom(src => src.AcademicTerm.Year.ToString()))
                .ForMember(dest => dest.AcademicTermSemester, opt => opt.MapFrom(src => src.AcademicTerm.Semester.ToString()));
            CreateMap<SubjectUpdateDTO, Subject>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => new SubjectCategory { Id = src.CategoryId }))
                .ForMember(dest => dest.AcademicTerm, opt => opt.MapFrom(src => new AcademicTerm { Id = src.AcademicTermId }));
            CreateMap<Subject, SubjectCreateDTO>().ReverseMap();
            CreateMap<CreateUpdateAcademicTermDTO, AcademicTerm>().ReverseMap();
            CreateMap<AcademicTerm, AcademicTermDTO>().ReverseMap();
            CreateMap<Class,CreateUpdateClassDTO>().ReverseMap();
            CreateMap<Class, ClassDTO>().ReverseMap();
        }
    }
}
