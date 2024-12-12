using EduPlanManager.Models.DTOs.Grade;
using EduPlanManager.Models.Entities;

namespace EduPlanManager.Models.ViewModels
{
    public class TeacherGradesViewModel
    {
        public List<StudentSubjectGradeDto> Grades { get; set; }
        public List<AcademicTerm> AcademicTerms { get; set; }
        public List<Class> Classes { get; set; }
        public List<Subject> Subjects { get; set; } 

    }
}
