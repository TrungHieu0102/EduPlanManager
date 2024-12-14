using EduPlanManager.Models.DTOs.Enrollment;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;

namespace EduPlanManager.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> EnrollSubjects(List<EnrollmentRequest> requests)
        {
            try
            {

                foreach (var request in requests)
                {

                    if (!await _unitOfWork.Enrollments.IsStudentInClass(request.StudentId, request.SubjectId))
                    {
                        throw new Exception("Sinh viên không thuộc lớp có môn học này.");
                    }
                    var subject = await _unitOfWork.Subjects.GetByIdAsync(request.SubjectId);
                    if (await _unitOfWork.Enrollments.HasDuplicateSubject(request.StudentId, request.SubjectId, subject.AcademicTermId))
                    {
                        throw new Exception("Sinh viên đã đăng ký môn học này.");
                    }
                    if (await _unitOfWork.Enrollments.HasConflictSchedule(request.StudentId, request.SubjectScheduleId))
                    {
                        throw new Exception("Sinh viên đã đăng ký môn học khác trong giờ này.");
                    }

                    var enrollment = new Enrollment
                    {
                        Id = Guid.NewGuid(),
                        StudentId = request.StudentId,
                        SubjectId = request.SubjectId,
                        SubjectScheduleId = request.SubjectScheduleId,
                        AcademicTermId = subject.AcademicTermId,
                        RegisteredAt = DateTime.UtcNow,
                        Status = EnrollmentStatus.Pending
                    };

                    await _unitOfWork.Enrollments.AddEnrollment(enrollment);
                    await _unitOfWork.CompleteAsync();

                }


                return new Result<string>
                {
                    IsSuccess = true,
                    Message = "Đăng ký môn học thành công."
                };
            }
            catch (Exception e)
            {
                return new Result<string>
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }
        public async Task<List<EnrollmentDetailResponse>> GetAllEnrollmentDetailsAsync()
        {
            return await _unitOfWork.Enrollments.GetAllEnrollmentDetailsAsync();
        }

        public async Task<ResultPage<EnrollmentListRespone>> GetEligibleSubjects(Guid studentId, int page, int pageSize, string searchTerm, DayOfWeekEnum? dayOfWeek, SessionEnum? session)
        {
            var subjectSchedules = await _unitOfWork.Enrollments.GetEligibleSubjects(studentId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                subjectSchedules = subjectSchedules.Where(s =>
                    s.SubjectCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    s.SubjectName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (dayOfWeek.HasValue)
            {
                subjectSchedules = subjectSchedules.Where(s => s.DayOfWeek == dayOfWeek.Value).ToList();
            }

            if (session.HasValue)
            {
                subjectSchedules = subjectSchedules.Where(s => s.Session == session.Value).ToList();
            }

            var totalCount = subjectSchedules.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var pagedSubjects = subjectSchedules
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ResultPage<EnrollmentListRespone>
            {
                IsSuccess = true,
                Data = pagedSubjects,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page
            };
        }

        public async Task<List<EnrollmentStudentListResponse>> GetEnrollmentsByStudentIdAsync(Guid studentId)
        {
            var enrollments = await _unitOfWork.Enrollments.GetEnrollmentsByStudentIdAsync(studentId);
            var respone = enrollments.Select(e => new EnrollmentStudentListResponse
            {
                EnrollmentId = e.Id,
                StudentId = e.StudentId,
                SubjectCode = e.Subject.Code,
                SubjectName = e.Subject.Name,
                DayOfWeek = e.SubjectSchedule.DayOfWeek.ToString(),
                Session = e.SubjectSchedule.Session.ToString(),
                StartTime = e.SubjectSchedule.StartTime.ToString(@"hh\:mm"),
                EndTime = e.SubjectSchedule.EndTime.ToString(@"hh\:mm"),
                RegisteredAt = e.RegisteredAt,
                Status = e.Status.ToString(),
            }).ToList();

            return respone;
        }
        public async Task<Result<List<EnrollmentRequestDto>>> GetAllEnrollmentRequestsAsync()
        {
            try
            {
                var enrollments = await _unitOfWork.Enrollments.GetAllEnrollmentRequestsAsync();

                var result = enrollments.Select(e => new EnrollmentRequestDto
                {
                    EnrollmentId = e.Id,
                    StudentName = e.Student.GetFullName(),
                    SubjectName = e.SubjectSchedule.Subjects
                    .FirstOrDefault(subject => subject.Id == e.SubjectId)?.Name ?? "N/A",
                    DayOfWeek = e.SubjectSchedule.DayOfWeek,
                    Session = e.SubjectSchedule.Session,
                    StartTime = e.SubjectSchedule.StartTime,
                    EndTime = e.SubjectSchedule.EndTime,
                    Status = e.Status.ToString()
                }).ToList();
                return new Result<List<EnrollmentRequestDto>>
                {
                    Data = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Result<List<EnrollmentRequestDto>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Result<bool>> ApproveEnrollmentAsync(Guid enrollmentId)
        {
            try
            {
                var enrollment = await _unitOfWork.Enrollments.GetDetailByIdAsync(enrollmentId) ?? throw new Exception("Không tìm thấy môn học");
                enrollment.Status = EnrollmentStatus.Approved;
                var studentSchedule = new StudentSchedule
                {
                    Id = Guid.NewGuid(),
                    StudentId = enrollment.StudentId,
                    SubjectId = enrollment.SubjectId,
                    DayOfWeek = enrollment.SubjectSchedule.DayOfWeek.ToString(),
                    Semester = enrollment.AcademicTerm.Semester,
                    Year = enrollment.AcademicTerm.Year,
                    StartTime = enrollment.SubjectSchedule.StartTime,
                    EndTime = enrollment.SubjectSchedule.EndTime,
                    AcademicTermId = enrollment.AcademicTermId,
                    Student = enrollment.Student,
                    Subject = enrollment.SubjectSchedule.Subjects.FirstOrDefault(s => s.Id == enrollment.SubjectId)
                };
                Guid summaryId = Guid.NewGuid();
                var summaryGreade = new SumaryGrade
                {
                    Id = summaryId,
                    StudentId = enrollment.StudentId,
                    SubjectId = enrollment.SubjectId,
                    Status = Status.Pass,
                    Summary = 0,
                    NeedsImprovement = false,
                    AcademicTermId = enrollment.AcademicTermId
                };
                await _unitOfWork.SumaryGrades.AddAsync(summaryGreade);
                await _unitOfWork.CompleteAsync();
                for (int i = 1; i < 4; i++)
                {
                    studentSchedule.Id = Guid.NewGuid();
                    var studentGrade = new Grade
                    {
                        Id = new Guid(),
                        StudentId = enrollment.StudentId,
                        SubjectId = enrollment.SubjectId,
                        AcademicTermId = enrollment.AcademicTermId,
                        Type = (GradeType)i,
                        SumaryGradeId = summaryId

                    };
                    await _unitOfWork.Grades.AddAsync(studentGrade);
                }
                await _unitOfWork.StudentSchedules.AddStudentScheduleAsync(studentSchedule);

                await _unitOfWork.CompleteAsync();
                return new Result<bool>
                {
                    IsSuccess = true,
                    Message = "Duyệt thành công"
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

        public async Task<Result<bool>> DeleteEnrollmentAsync(Guid enrollmentId, Guid studentId)
        {
            try
            {
                var enrollment = await _unitOfWork.Enrollments.GetEnrollmentByIdAsync(enrollmentId)
                    ?? throw new Exception("Không tìm thấy mã đăng ký");
                if (enrollment.StudentId != studentId)
                {

                    throw new Exception("Không thể hủy đăng ký của sinh viên khác");
                }
                if (enrollment.Status == EnrollmentStatus.Approved)
                {
                    throw new Exception("Không thể hủy môn học đã được duyệt");
                }
                _unitOfWork.Enrollments.Delete(enrollment);
                await _unitOfWork.CompleteAsync();
                return new Result<bool>
                {
                    IsSuccess = true,
                    Message = "Hủy thành công"
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
    }
}
