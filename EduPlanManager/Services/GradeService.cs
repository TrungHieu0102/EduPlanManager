using EduPlanManager.Models.DTOs.Grade;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;
using Microsoft.AspNetCore.Identity;

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
        /// <summary>
        /// Lấy danh sách điểm của sinh viên theo ID người dùng.
        /// Phương thức này sẽ truy xuất dữ liệu điểm của sinh viên và trả về dưới dạng danh sách DTO.
        /// </summary>
        /// <param name="userId">ID của sinh viên</param>
        /// <returns>Danh sách điểm của sinh viên dưới dạng GradeDto</returns>
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

        /// <summary>
        /// Lấy danh sách điểm của sinh viên và nhóm theo học kỳ.
        /// Phương thức này sẽ tính toán điểm tổng kết của sinh viên theo môn học và học kỳ.
        /// Điểm được tính theo công thức, và trạng thái (Đạt/Không đạt) được xác định dựa trên tổng điểm.
        /// </summary>
        /// <param name="studentId">ID của sinh viên</param>
        /// <returns>Danh sách điểm theo học kỳ với các thông tin về môn học và trạng thái</returns>
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
        /// <summary>
        /// Thêm hoặc cập nhật điểm cho sinh viên trong môn học.
        /// Phương thức này kiểm tra quyền của giảng viên, sau đó thêm hoặc cập nhật điểm của sinh viên cho môn học trong học kỳ tương ứng.
        /// </summary>
        /// <param name="teacherId">ID của giảng viên</param>
        /// <param name="studentId">ID của sinh viên</param>
        /// <param name="subjectId">ID của môn học</param>
        /// <param name="score">Điểm số cần thêm hoặc cập nhật</param>
        /// <param name="type">Loại điểm (Giữa kỳ, Cuối kỳ, Thưởng)</param>
        /// <returns>Thông báo kết quả việc thêm hoặc cập nhật điểm</returns>
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
        /// <summary>
        /// Lấy danh sách các môn học và học kỳ mà giảng viên phụ trách.
        /// Phương thức này truy xuất các môn học mà giảng viên dạy và các học kỳ tương ứng.
        /// </summary>
        /// <param name="teacherId">ID của giảng viên</param>
        /// <returns>Danh sách các môn học và học kỳ của giảng viên</returns>
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

        /// <summary>
        /// Lấy danh sách điểm của sinh viên mà giảng viên phụ trách theo nhiều tiêu chí tìm kiếm.
        /// Phương thức này cho phép giảng viên lọc điểm sinh viên theo môn học, học kỳ, tên sinh viên hoặc lớp học.
        /// </summary>
        /// <param name="teacherId">ID của giảng viên</param>
        /// <param name="subjectId">ID của môn học (có thể null)</param>
        /// <param name="studentName">Tên sinh viên (có thể null)</param>
        /// <param name="academicTermId">ID học kỳ (có thể null)</param>
        /// <param name="classId">ID lớp học (có thể null)</param>
        /// <returns>Danh sách điểm của sinh viên theo các tiêu chí lọc</returns
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
