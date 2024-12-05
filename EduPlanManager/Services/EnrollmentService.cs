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

        public async Task<string> EnrollSubjects(List<EnrollmentRequest> requests)
        {
            Guid defaultAcademicTermId = Guid.Parse("1CCB0087-DBD0-4BFA-B990-3F3CD481ACB0");

            foreach (var request in requests)
            {
                var academicTermId = request.AcademicTermId == Guid.Empty ? defaultAcademicTermId : request.AcademicTermId;

                if (!await _unitOfWork.Enrollments.IsStudentInClass(request.StudentId, request.SubjectId))
                {
                    return "Sinh viên không thuộc lớp có môn học này.";
                }

                if (await _unitOfWork.Enrollments.HasConflictSchedule(request.StudentId, request.SubjectScheduleId))
                {
                    return "Sinh viên đã đăng ký môn học khác trong giờ này.";
                }

                if (await _unitOfWork.Enrollments.HasDuplicateSubject(request.StudentId, request.SubjectId, academicTermId))
                {
                    return "Sinh viên đã đăng ký môn học này.";
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

            return "Đăng ký các môn học thành công!";
        }
        public async Task<List<EnrollmentDetailResponse>> GetAllEnrollmentDetailsAsync()
        {
            return await _unitOfWork.Enrollments.GetAllEnrollmentDetailsAsync();
        }

        public async Task<ResultPage<EnrollmentListRespone>> GetEligibleSubjects(Guid studentId, int page, int pageSize)
        {
            var subjectSchedules = await _unitOfWork.Enrollments.GetEligibleSubjects(studentId);
            // Tính toán tổng số bản ghi và tổng số trang
            var totalCount = subjectSchedules.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Lấy các môn học phân trang
            var pagedSubjects = subjectSchedules
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Trả về kết quả phân trang với dữ liệu
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
    }
}
