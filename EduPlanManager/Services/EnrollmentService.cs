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

        // Constructor: Khởi tạo EnrollmentService với đối tượng IUnitOfWork.
        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Xử lý đăng ký môn học cho sinh viên.
        /// - Kiểm tra tính hợp lệ của từng yêu cầu đăng ký (kiểm tra xem sinh viên có thuộc lớp, đã đăng ký môn học, hay có xung đột giờ học).
        /// - Nếu tất cả các điều kiện hợp lệ, tạo đối tượng Enrollment mới và lưu vào cơ sở dữ liệu.
        /// - Trả về kết quả với thông báo thành công hoặc thất bại.
        /// </summary>
        /// <param name="requests">Danh sách yêu cầu đăng ký môn học của sinh viên.</param>
        /// <returns>Result<string>: Kết quả đăng ký môn học (thành công hoặc thất bại).</returns>
        public async Task<Result<string>> EnrollSubjects(List<EnrollmentRequest> requests)
        {
            try
            {
                foreach (var request in requests)
                {
                    // Kiểm tra xem sinh viên có thuộc lớp này không.
                    if (!await _unitOfWork.Enrollments.IsStudentInClass(request.StudentId, request.SubjectId))
                    {
                        throw new Exception("Sinh viên không thuộc lớp có môn học này.");
                    }

                    var subject = await _unitOfWork.Subjects.GetByIdAsync(request.SubjectId);

                    // Kiểm tra sinh viên đã đăng ký môn học này chưa.
                    if (await _unitOfWork.Enrollments.HasDuplicateSubject(request.StudentId, request.SubjectId, subject.AcademicTermId))
                    {
                        throw new Exception("Sinh viên đã đăng ký môn học này.");
                    }

                    // Kiểm tra xem sinh viên có xung đột giờ học không.
                    if (await _unitOfWork.Enrollments.HasConflictSchedule(request.StudentId, request.SubjectScheduleId))
                    {
                        throw new Exception("Sinh viên đã đăng ký môn học khác trong giờ này.");
                    }

                    // Tạo và thêm đối tượng Enrollment mới vào cơ sở dữ liệu.
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

        /// <summary>
        /// Lấy tất cả các chi tiết đăng ký môn học.
        /// - Trả về danh sách các chi tiết đăng ký môn học từ cơ sở dữ liệu.
        /// </summary>
        /// <returns>Danh sách chi tiết đăng ký môn học.</returns>
        public async Task<List<EnrollmentDetailResponse>> GetAllEnrollmentDetailsAsync()
        {
            return await _unitOfWork.Enrollments.GetAllEnrollmentDetailsAsync();
        }

        /// <summary>
        /// Lấy danh sách các môn học có thể đăng ký của sinh viên.
        /// - Thực hiện phân trang và tìm kiếm môn học dựa trên các tiêu chí (ngày học, ca học, mã môn học, tên môn học).
        /// - Trả về kết quả phân trang của danh sách các môn học đủ điều kiện.
        /// </summary>
        /// <param name="studentId">ID của sinh viên.</param>
        /// <param name="page">Số trang hiện tại.</param>
        /// <param name="pageSize">Số lượng môn học mỗi trang.</param>
        /// <param name="searchTerm">Từ khóa tìm kiếm (tên hoặc mã môn học).</param>
        /// <param name="dayOfWeek">Ngày trong tuần.</param>
        /// <param name="session">Ca học.</param>
        /// <returns>Kết quả phân trang các môn học đủ điều kiện đăng ký.</returns>
        public async Task<ResultPage<EnrollmentListRespone>> GetEligibleSubjects(Guid studentId, int page, int pageSize, string searchTerm, DayOfWeekEnum? dayOfWeek, SessionEnum? session)
        {
            var subjectSchedules = await _unitOfWork.Enrollments.GetEligibleSubjects(studentId);

            // Tìm kiếm theo mã hoặc tên môn học.
            if (!string.IsNullOrEmpty(searchTerm))
            {
                subjectSchedules = subjectSchedules.Where(s =>
                    s.SubjectCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    s.SubjectName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Lọc theo ngày trong tuần nếu có.
            if (dayOfWeek.HasValue)
            {
                subjectSchedules = subjectSchedules.Where(s => s.DayOfWeek == dayOfWeek.Value).ToList();
            }

            // Lọc theo ca học nếu có.
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

        /// <summary>
        /// Lấy danh sách các môn học đã đăng ký của sinh viên.
        /// - Trả về danh sách các môn học mà sinh viên đã đăng ký với thông tin chi tiết như mã môn, tên môn, ca học, giờ học, v.v.
        /// </summary>
        /// <param name="studentId">ID của sinh viên.</param>
        /// <returns>Danh sách các môn học mà sinh viên đã đăng ký.</returns>
        public async Task<List<EnrollmentStudentListResponse>> GetEnrollmentsByStudentIdAsync(Guid studentId)
        {
            var enrollments = await _unitOfWork.Enrollments.GetEnrollmentsByStudentIdAsync(studentId);
            var response = enrollments.Select(e => new EnrollmentStudentListResponse
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

            return response;
        }

        /// <summary>
        /// Lấy tất cả các yêu cầu đăng ký môn học.
        /// - Trả về danh sách các yêu cầu đăng ký môn học với thông tin chi tiết về sinh viên, môn học, giờ học, v.v.
        /// </summary>
        /// <returns>Danh sách các yêu cầu đăng ký môn học.</returns>
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

        /// <summary>
        /// Duyệt yêu cầu đăng ký môn học của sinh viên.
        /// - Cập nhật trạng thái của yêu cầu đăng ký là "Approved".
        /// - Tạo lịch học cho sinh viên và thêm vào bảng StudentSchedules.
        /// - Tạo bảng SummaryGrade và Grade cho sinh viên.
        /// </summary>
        /// <param name="enrollmentId">ID yêu cầu đăng ký.</param>
        /// <returns>Kết quả duyệt yêu cầu đăng ký môn học.</returns>
        public async Task<Result<bool>> ApproveEnrollmentAsync(Guid enrollmentId)
        {
            try
            {
                var enrollment = await _unitOfWork.Enrollments.GetDetailByIdAsync(enrollmentId) ?? throw new Exception("Không tìm thấy môn học");
                enrollment.Status = EnrollmentStatus.Approved;

                // Tạo lịch học cho sinh viên.
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

                // Tạo bảng SummaryGrade và Grade cho sinh viên.
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

                // Thêm lịch học của sinh viên vào cơ sở dữ liệu.
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

        /// <summary>
        /// Xóa đăng ký môn học của sinh viên.
        /// - Kiểm tra quyền hạn của sinh viên và trạng thái của đăng ký trước khi xóa.
        /// - Nếu đăng ký đã được duyệt, không thể xóa.
        /// </summary>
        /// <param name="enrollmentId">ID đăng ký môn học.</param>
        /// <param name="studentId">ID sinh viên.</param>
        /// <returns>Kết quả xóa đăng ký môn học.</returns>
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

                // Kiểm tra trạng thái của đăng ký.
                if (enrollment.Status == EnrollmentStatus.Approved)
                {
                    throw new Exception("Không thể hủy môn học đã được duyệt");
                }

                // Xóa đăng ký môn học và hoàn tất thay đổi.
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
