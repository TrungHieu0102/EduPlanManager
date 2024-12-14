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
        //Dùng để student có thể xem được điểm các môn học của mình
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
        //Dùng để sinh viên xem kết quả các môn học đạt hay không đạt, kết quả toàn học kỳ 
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
                            if (totalScore > 10) totalScore = 10;
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
       //Thêm điểm cho sinh viên
        public async Task<Result<bool>> AddGradeAsync(Guid teacherId, Guid studentId, Guid subjectId, float score, GradeType type)
        {
            try
            {
                var teacher = await _userManager.FindByIdAsync(teacherId.ToString());

                if (teacher == null || !await _userManager.IsInRoleAsync(teacher, "Teacher"))
                {
                    throw new Exception("Teacher not found");
                }
                var subject = await _unitOfWork.Subjects.GetByIdAsync(subjectId) ?? throw new Exception("Không tìm thấy môn học");
                var teacherSubjects = await _unitOfWork.Subjects.GetAllSubjectByUserId(teacherId);
                var grades = await _unitOfWork.Grades.GetAllGrade();
                var filteredGrades = grades
                                .Where(grade => teacherSubjects.
                                 Any(ts => ts.Id == grade.SubjectId && ts.AcademicTermId == grade.AcademicTermId))
                                .ToList();
                if (!filteredGrades.Any())
                {
                    throw new Exception("Bạn không phụ trách lớp này");
                }

                var gradeUpdate = await _unitOfWork.Grades.GetGradeWithoutId(studentId, type, subjectId, subject.AcademicTermId)
                                  ?? throw new Exception("Không thể tìm thấy điểm");
                gradeUpdate.Score = score;
                _unitOfWork.Grades.Update(gradeUpdate);

                var grade = await _unitOfWork.Grades.GetGradesBySubjectAndStudent(studentId, subjectId, subject.AcademicTermId);
                var midtermScore = grade.Where(g => g.Type == GradeType.Midterm).Sum(g => g.Score);
                var finalScore = grade.Where(g => g.Type == GradeType.Final).Sum(g => g.Score);
                var bonusScore = grade.Where(g => g.Type == GradeType.Bonus).Sum(g => g.Score);

                var totalScore = Math.Round((midtermScore + bonusScore) * 0.4 + finalScore * 0.6, 2);
                if (totalScore > 10) totalScore = 10;

                var summaryGrade = await _unitOfWork.SumaryGrades.GetSummaryGrade(studentId, subjectId, subject.AcademicTermId)
                                   ?? throw new Exception("Không tìm thấy SummaryGrade");

                summaryGrade.Summary = (float)totalScore;
                summaryGrade.Status = totalScore < 4 ? Status.Fail : Status.Pass;

                _unitOfWork.SumaryGrades.Update(summaryGrade);

                await _unitOfWork.CompleteAsync();

                return new Result<bool>
                {
                    Message = "Điểm đã được cập nhật thành công",
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
        //Lọc dữ liệu để lấy danh sách sinh viên có môn học do teacher phụ trách
        public async Task<(List<Subject> Subjects, List<AcademicTerm> AcademicTerms)> GetTeacherSubjectsAndAcademicTermsAsync(Guid teacherId)
        {
            var teacherSubjects = await _unitOfWork.Subjects.GetAllSubjectByUserId(teacherId);

            if (teacherSubjects == null || !teacherSubjects.Any())
            {
                return (new List<Subject>(), new List<AcademicTerm>());
            }


            var teacherAcademicTerms = teacherSubjects
                                        .Select(s => s.AcademicTerm)
                                        .Distinct()
                                        .ToList();

            return (teacherSubjects, teacherAcademicTerms);
        }
        public async Task<Result<List<StudentSubjectGradeDto>>> GetTeacherResponsibleGradesAsync(Guid teacherId, Guid? subjectId, string? studentName = null, Guid? academicTermId = null, Guid? classId = null)
        {
            try
            {
                var teacherSubjects = await _unitOfWork.Subjects.GetAllSubjectByUserId(teacherId);
                if (teacherSubjects == null || !teacherSubjects.Any())
                {
                    throw new Exception("Không tìm thấy môn học");
                }
                var grades = await _unitOfWork.Grades.GetAllGrade();
                var filteredGrades = grades
                                .Where(grade => teacherSubjects.
                                 Any(ts => ts.Id == grade.SubjectId && ts.AcademicTermId == grade.AcademicTermId))
                                .ToList();
                if (!filteredGrades.Any())
                {
                    throw new Exception("Không tìm thấy điểm phù hợp");
                }
                if (!string.IsNullOrEmpty(studentName))
                {
                    filteredGrades = filteredGrades.Where(g => g.Student.GetFullName().Contains(studentName, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (academicTermId.HasValue)
                {
                    filteredGrades = filteredGrades.Where(g => g.AcademicTermId == academicTermId.Value).ToList();
                }

                if (subjectId.HasValue)
                {
                    filteredGrades = filteredGrades.Where(g => g.SubjectId == subjectId.Value).ToList();
                }
                var groupedGrades = filteredGrades
              .GroupBy(g => new { g.StudentId, g.SubjectId })
              .Select(g => new StudentSubjectGradeDto
              {
                  StudentFullName = g.First().Student.GetFullName(),
                  SubjectName = g.First().Subject.Name,
                  MidtermScore = g.FirstOrDefault(x => x.Type == GradeType.Midterm)?.Score.ToString() ?? "Trống",
                  FinalScore = g.FirstOrDefault(x => x.Type == GradeType.Final)?.Score.ToString() ?? "Trống",
                  BonusScore = g.FirstOrDefault(x => x.Type == GradeType.Bonus)?.Score.ToString() ?? "Trống",
                  StudentId = g.First().StudentId.ToString(),
                  SubjectId = g.First().SubjectId.ToString()
              })
              .ToList();

                return new Result<List<StudentSubjectGradeDto>>
                {
                    Data = groupedGrades,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Result<List<StudentSubjectGradeDto>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
