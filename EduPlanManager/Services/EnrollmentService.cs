using EduPlanManager.Models.DTOs.Enrollment;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;
using NuGet.Protocol.Core.Types;
using PagedList.Core;

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
                Guid defaultAcademicTermId = Guid.Parse("1CCB0087-DBD0-4BFA-B990-3F3CD481ACB0");

                foreach (var request in requests)
                {
                    var academicTermId = request.AcademicTermId == Guid.Empty ? defaultAcademicTermId : request.AcademicTermId;

                    if (!await _unitOfWork.Enrollments.IsStudentInClass(request.StudentId, request.SubjectId))
                    {
                        throw new Exception("Sinh viên không thuộc lớp có môn học này.");
                    }

                    if (await _unitOfWork.Enrollments.HasConflictSchedule(request.StudentId, request.SubjectScheduleId))
                    {
                        throw new Exception("Sinh viên đã đăng ký môn học khác trong giờ này.");
                    }

                    if (await _unitOfWork.Enrollments.HasDuplicateSubject(request.StudentId, request.SubjectId, academicTermId))
                    {
                        throw new Exception("Sinh viên đã đăng ký môn học này.");
                    }

                    var enrollment = new Enrollment
                    {
                        Id = Guid.NewGuid(),
                        StudentId = request.StudentId,
                        SubjectId = request.SubjectId,
                        SubjectScheduleId = request.SubjectScheduleId,
                        AcademicTermId = academicTermId,
                        RegisteredAt = DateTime.UtcNow,
                        Status = EnrollmentStatus.Pending
                    };

                    await _unitOfWork.Enrollments.AddEnrollment(enrollment);
                }

                await _unitOfWork.CompleteAsync();

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
            return enrollments.Select(e => new EnrollmentStudentListResponse
            {
                EnrollmentId = e.Id,
                StudentId = e.StudentId,
                SubjectCode = e.Subject.Code,
                SubjectName = e.Subject.Name,
                DayOfWeek = e.SubjectSchedule.DayOfWeek.ToString(),
                Session = e.SubjectSchedule.Session.ToString(),
                StartTime = e.SubjectSchedule.StartTime.ToString(@"hh\:mm"),
                EndTime = e.SubjectSchedule.EndTime.ToString(@"hh\:mm"),
                RegisteredAt = e.RegisteredAt
            }).ToList();
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
    }
}
