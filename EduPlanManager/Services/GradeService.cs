using EduPlanManager.Models.DTOs.Grade;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Services
{
    public class GradeService : IGradeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public GradeService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<Result<List<GradeDto>>> GetGradeByUserID(Guid userId)
        {
            try
            {
                var grades = await _unitOfWork.Grades.GetGradeByUserID(userId);
                var gradeDtos = new List<GradeDto>();
                foreach (var grade in grades)
                {
                    gradeDtos.Add(new GradeDto
                    {
                        StudentName = grade.Student.GetFullName(),
                        SubjectName = grade.Subject.Name,
                        AcademicYear = grade.AcademicTerm.Year,
                        AcademicSemester = grade.AcademicTerm.Semester,
                        GradeValue = grade.Score,
                        GradeType = grade.Type.ToString(),
                    });
                }
                return new Result<List<GradeDto>>
                {
                    Data = gradeDtos,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Result<List<GradeDto>>
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }

        }

        public async Task<List<SemesterGradeDto>> GetStudentGradesGroupedBySemesterAsync(Guid studentId)
        {
            var grades = await _unitOfWork.Grades.GetGradeByUserID(studentId);

            var semesterGrades = grades
                .GroupBy(g => new { g.AcademicTerm.Year, g.AcademicTerm.Semester })
                .Select(semesterGroup => new SemesterGradeDto
                {
                    AcademicYear = semesterGroup.Key.Year,
                    AcademicSemester = semesterGroup.Key.Semester,
                    SubjectGrades = semesterGroup
                        .GroupBy(g => g.SubjectId)
                        .Select(subjectGroup =>
                        {
                            var subjectName = subjectGroup.First().Subject.Name;
                            var midtermScore = subjectGroup
                                .Where(g => g.Type == GradeType.Midterm)
                                .Sum(g => g.Score);

                            var finalScore = subjectGroup
                                .Where(g => g.Type == GradeType.Final)
                                .Sum(g => g.Score);

                            var bonusScore = subjectGroup
                                .Where(g => g.Type == GradeType.Bonus)
                                .Sum(g => g.Score);

                            var totalScore = Math.Round((midtermScore + bonusScore) * 0.4 + finalScore * 0.6, 2);

                            return new SubjectGradeDto
                            {
                                SubjectName = subjectName,
                                TotalScore = totalScore,
                                Status = totalScore < 4 ? "Không đạt" : "Đạt"
                            };
                        })
                        .ToList()
                })
                .OrderBy(sg => sg.AcademicYear)
                .ThenBy(sg => sg.AcademicSemester)
                .ToList();

            return semesterGrades;
        }

        public async Task<Result<bool>> AddGradeAsync(Guid teacherId, Guid studentId, Guid subjectId, float score, GradeType type)
        {
            try
            {
                var teacher = await _userManager.FindByIdAsync(teacherId.ToString());
                var subject = await _unitOfWork.Subjects.GetByIdAsync(subjectId) ?? throw new Exception("Không tìm thấy môn học");

                if (teacher == null || !await _userManager.IsInRoleAsync(teacher, "Teacher"))
                {
                    throw new Exception("Teacher not found");
                }
                var teacherClass = await _unitOfWork.Classes.GetAllClassByUserIdAsync(teacherId);
                var studentClass = await _unitOfWork.Classes.GetAllClassByUserIdAsync(studentId);
                var commonClasses = teacherClass.Intersect(studentClass).ToList();
                if (!commonClasses.Any())
                {
                    throw new Exception("Giáo viên và học sinh không thuộc cùng lớp học.");
                }
                var subjectInCommon = commonClasses.SelectMany(c => c.Subjects).Any(s => s.Id == subjectId);
                if (!subjectInCommon)
                {
                    throw new Exception("Giáo viên và học sinh không có cùng môn học.");
                }
                var grade = new Grade
                {
                    Id = Guid.NewGuid(),
                    StudentId = studentId,
                    SubjectId = subjectId,
                    Score = score,
                    Type = type,
                    AcademicTermId = subject.AcademicTermId,
                };
                await _unitOfWork.Grades.AddAsync(grade);
                await _unitOfWork.CompleteAsync();
                return new Result<bool>
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Result<bool>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }

        }
        public async Task<List<StudentSubjectGradeDto>> GetTeacherResponsibleGradesAsync(Guid teacherId, Guid? subjectId, string? studentName = null, Guid? academicTermId = null, Guid? classId = null)
        {
            var teacherClasses = await _unitOfWork.Classes.GetAllClassByUserIdAsync(teacherId);
            if (teacherClasses == null || !teacherClasses.Any())
            {
                return new List<StudentSubjectGradeDto>();
            }

            var teacherSubjects = teacherClasses
                                  .SelectMany(c => c.Subjects)
                                  .Distinct()
                                  .ToList();

            var grades = await _unitOfWork.Grades.GetStudentGradeOnTeacher(teacherClasses, teacherSubjects);

            if (grades == null || !grades.Any())
            {
                return new List<StudentSubjectGradeDto>();
            }

            if (!string.IsNullOrEmpty(studentName))
            {
                grades = grades.Where(g => g.Student.GetFullName().Contains(studentName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (academicTermId.HasValue)
            {
                grades = grades.Where(g => g.AcademicTermId == academicTermId.Value).ToList();
            }

            if (classId.HasValue)
            {
                grades = grades.Where(g => g.Student.Classes.Any(c => c.Id == classId.Value)).ToList();
            }
            if (subjectId.HasValue)
            {
                grades = grades.Where(g => g.SubjectId == subjectId.Value).ToList();
            }
            var groupedGrades = grades
                .GroupBy(g => new { g.StudentId, g.SubjectId })
                .Select(g => new StudentSubjectGradeDto
                {
                    StudentFullName = g.First().Student.GetFullName(),
                    SubjectName = g.First().Subject.Name,
                    MidtermScore = g.FirstOrDefault(x => x.Type == GradeType.Midterm)?.Score.ToString() ?? "Trống",
                    FinalScore = g.FirstOrDefault(x => x.Type == GradeType.Final)?.Score.ToString() ?? "Trống",
                    BonusScore = g.FirstOrDefault(x => x.Type == GradeType.Bonus)?.Score.ToString() ?? "Trống"
                })
                .ToList();

            return groupedGrades;
        }

        public async Task<(List<Subject> Subjects, List<AcademicTerm> AcademicTerms, List<Class> Classes)> GetTeacherSubjectsAndAcademicTermsAsync(Guid teacherId)
        {
            var teacherClasses = await _unitOfWork.Classes.GetAllClassByUserIdAsync(teacherId);

            if (teacherClasses == null || !teacherClasses.Any())
            {
                return (new List<Subject>(), new List<AcademicTerm>(), new List<Class>());
            }

            var teacherSubjects = teacherClasses
                                    .SelectMany(c => c.Subjects)
                                    .Distinct()
                                    .ToList();

            var teacherAcademicTerms = teacherClasses
                                        .SelectMany(c => c.Subjects)
                                        .Select(s => s.AcademicTerm)
                                        .Distinct()
                                        .ToList();

            return (teacherSubjects, teacherAcademicTerms, teacherClasses);
        }

    }
}
