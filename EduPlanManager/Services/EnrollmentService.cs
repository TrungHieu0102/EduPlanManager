using EduPlanManager.Models.DTOs.Enrollment;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;
using NuGet.Protocol.Core.Types;

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
            // Định nghĩa AcademicTermId mặc định
            Guid defaultAcademicTermId = Guid.Parse("1CCB0087-DBD0-4BFA-B990-3F3CD481ACB0");

            // Duyệt qua từng request trong danh sách
            foreach (var request in requests)
            {
                // Sử dụng AcademicTermId mặc định nếu không có giá trị trong request
                var academicTermId = request.AcademicTermId == Guid.Empty ? defaultAcademicTermId : request.AcademicTermId;

                // Kiểm tra: Sinh viên có thuộc lớp có môn học không
                if (!await _unitOfWork.Enrollments.IsStudentInClass(request.StudentId, request.SubjectId))
                {
                    return "Sinh viên không thuộc lớp có môn học này.";
                }

                // Kiểm tra: Mỗi giờ học chỉ được đăng ký một môn
                if (await _unitOfWork.Enrollments.HasConflictSchedule(request.StudentId, request.SubjectScheduleId))
                {
                    return "Sinh viên đã đăng ký môn học khác trong giờ này.";
                }

                // Kiểm tra: Không được đăng ký trùng môn học
                if (await _unitOfWork.Enrollments.HasDuplicateSubject(request.StudentId, request.SubjectId, academicTermId))
                {
                    return "Sinh viên đã đăng ký môn học này.";
                }

                // Thêm mới đăng ký
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

                // Thêm vào cơ sở dữ liệu
                await _unitOfWork.Enrollments.AddEnrollment(enrollment);
            }

            // Lưu thay đổi
            await _unitOfWork.CompleteAsync();

            return "Đăng ký các môn học thành công!";
        }


        public async Task<List<EnrollmentListRespone>> GetEligibleSubjects(Guid studentId)
        {
            return await _unitOfWork.Enrollments.GetEligibleSubjects(studentId);
        }
    }
}
