using EduPlanManager.Models.DTOs.Grade;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Services
{
    public class GradeService : IGradeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public GradeService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
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

    }
}
