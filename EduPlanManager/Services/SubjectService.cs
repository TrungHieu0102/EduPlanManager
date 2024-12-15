using AutoMapper;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.DTOs.Subject;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace EduPlanManager.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SubjectService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        /// <summary>
        /// Tìm kiếm các môn học theo từ khóa, kỳ học, năm học với phân trang.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm (tên hoặc mã môn học).</param>
        /// <param name="semester">Kỳ học.</param>
        /// <param name="year">Năm học.</param>
        /// <param name="pageNumber">Số trang hiện tại.</param>
        /// <param name="pageSize">Số môn học mỗi trang.</param>
        /// <returns>Danh sách các môn học tìm được với thông tin phân trang.</returns>
        public async Task<ResultPage<SubjectDTO>> SearchSubjectsAsync(string searchTerm, int? semester, int? year, int pageNumber, int pageSize)
        {
            try
            {
                var query = _unitOfWork.Subjects.GetQueryable();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(s => s.Name.StartsWith(searchTerm) || s.Code.StartsWith(searchTerm));
                }
                if (semester.HasValue)
                {
                    query = query.Where(s => s.AcademicTerm.Semester == semester);
                }
                if (year.HasValue)
                {
                    query = query.Where(s => s.AcademicTerm.Year == year);
                }

                int totalSubjects = await query.CountAsync();

                var totalPages = (int)Math.Ceiling((double)totalSubjects / pageSize);

                var subjects = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var subjectDto = _mapper.Map<IEnumerable<SubjectDTO>>(subjects);

                var result = new ResultPage<SubjectDTO>
                {
                    IsSuccess = true,
                    Data = subjectDto,
                    TotalCount = totalSubjects,
                    TotalPages = totalPages,
                    CurrentPage = pageNumber
                };

                return result;
            }
            catch (Exception ex)
            {
                return new ResultPage<SubjectDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        /// <summary>
        /// Lấy chi tiết của một môn học theo ID.
        /// </summary>
        /// <param name="id">ID của môn học.</param>
        /// <returns>Thông tin chi tiết của môn học.</returns>
        public async Task<Result<SubjectDetailDTO>> GetSubjectWithDetailsAsync(Guid id)
        {
            try
            {
                var subject = await _unitOfWork.Subjects.GetSubjectWithDetailsAsync(id);
                var result = _mapper.Map<SubjectDetailDTO>(subject);
                return new Result<SubjectDetailDTO>
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new Result<SubjectDetailDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        /// <summary>
        /// Lấy thông tin của một môn học theo ID.
        /// </summary>
        /// <param name="id">ID của môn học.</param>
        /// <returns>Môn học với thông tin cơ bản.</returns>
        public async Task<Result<Subject>> GetSubject(Guid id)
        {
            try
            {
                var subject = await _unitOfWork.Subjects.GetByIdAsync(id);
                return new Result<Subject>
                {
                    IsSuccess = true,
                    Data = subject
                };
            }
            catch (Exception ex)
            {
                return new Result<Subject>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        /// <summary>
        /// Đếm số lượng môn học thỏa mãn điều kiện tìm kiếm.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm (tên hoặc mã môn học).</param>
        /// <param name="semester">Kỳ học.</param>
        /// <param name="year">Năm học.</param>
        /// <returns>Số lượng môn học thỏa mãn các điều kiện tìm kiếm.</returns>
        public async Task<int> CountSubjectsAsync(string searchTerm, int? semester, int? year)
        {
            return await _unitOfWork.Subjects.GetTotalSubjectsAsync(searchTerm, semester, year);
        }
        /// <summary>
        /// Tìm kiếm môn học theo tên hoặc mã môn học.
        /// </summary>
        /// <param name="term">Từ khóa tìm kiếm.</param>
        /// <returns>Danh sách các môn học tìm được.</returns>  
        public async Task<Result<IEnumerable<SubjectDTO>>> SearchSubjectsByNameOrCodeAsync(string term)
        {
            try
            {
                var listSubject = await _unitOfWork.Subjects
               .FindAsync(s => s.Name.StartsWith(term) || s.Code.StartsWith(term))
               .Take(10).ToListAsync();
                var subjectDto = _mapper.Map<IEnumerable<SubjectDTO>>(listSubject);
                return new Result<IEnumerable<SubjectDTO>>
                {
                    IsSuccess = true,
                    Data = subjectDto
                };
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<SubjectDTO>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }

        }
        /// <summary>
        /// Xóa một môn học theo ID.
        /// </summary>
        /// <param name="id">ID của môn học cần xóa.</param>
        /// <returns>Kết quả xóa môn học (thành công hay thất bại).</returns>
        public async Task<Result<bool>> DeleteSubjectById(Guid id)
        {
            try
            {
                var subject = await _unitOfWork.Subjects.GetByIdAsync(id);
                _unitOfWork.Subjects.Delete(subject);
                if (await _unitOfWork.CompleteAsync() == 0)
                    throw new Exception("Không có thay đổi nào được lưu vào cơ sở dữ liệu.");
                return new Result<bool>
                {
                    IsSuccess = true,
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
        /// Cập nhật thông tin môn học.
        /// </summary>
        /// <param name="subject">Môn học với các thông tin cập nhật.</param>
        /// <returns>Môn học sau khi được cập nhật.</returns>
        public async Task<Result<Subject>> UpdateSubject(Subject subject)
        {
            try
            {
                var existingSubject = await _unitOfWork.Subjects.GetByIdAsync(subject.Id) ?? throw new Exception("Không tìm thấy môn học");

                existingSubject.Code = subject.Code;
                existingSubject.Name = subject.Name;
                existingSubject.StartDate = subject.StartDate;
                existingSubject.EndDate = subject.EndDate;
                existingSubject.LessonsPerDay = subject.LessonsPerDay;
                existingSubject.CategoryId = subject.CategoryId;
                existingSubject.AcademicTermId = subject.AcademicTermId;
                _unitOfWork.Subjects.Update(existingSubject);
                var result = await _unitOfWork.CompleteAsync();
                return new Result<Subject>
                {
                    IsSuccess = true,
                    Data = existingSubject
                };
            }
            catch (Exception ex)
            {
                return new Result<Subject>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        /// <summary>
        /// Xóa nhiều môn học theo danh sách ID.
        /// </summary>
        /// <param name="ids">Danh sách các ID môn học cần xóa.</param>
        /// <returns>Không trả về kết quả cụ thể, chỉ đảm bảo xóa các môn học.</returns>
        public async Task DeleteSubjectsAsync(List<Guid> ids)
        {
            var subjects = await _unitOfWork.Subjects.GetSubjectsByIdsAsync(ids);
            if (subjects.Count != 0)
            {
                await _unitOfWork.Subjects.DeleteSubjectsAsync(subjects);
            }
        }

        /// <summary>
        /// Tạo mới một môn học.
        /// </summary>
        /// <param name="subjectCreateDTO">Thông tin môn học mới.</param>
        /// <returns>Môn học vừa tạo với thông tin trả về.</returns>
        public async Task<Result<SubjectDTO>> CreateSubjectAsync(SubjectCreateDTO subjectCreateDTO)
        {
            try
            {
                var isExists = await _unitOfWork.Subjects.IsSubjectExistsInformation(subjectCreateDTO.Code, subjectCreateDTO.AcademicTermId, subjectCreateDTO.TeacherId);
                if (isExists) throw new Exception("Môn học đã tồn tại. Vui lòng kiểm tra lại thông tin");
                subjectCreateDTO.Id = Guid.NewGuid();
                var subject = _mapper.Map<Subject>(subjectCreateDTO);
                subject.Code = subject.Code.ToUpper();
                _unitOfWork.Subjects.Add(subject);
                await _unitOfWork.CompleteAsync();
                var result = _mapper.Map<SubjectDTO>(subject);
                return new Result<SubjectDTO>
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new Result<SubjectDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        /// <summary>
        /// Lấy danh sách các môn học có liên kết với lớp học cụ thể.
        /// </summary>
        /// <param name="isHaveClass">Xác định môn học có lớp học hay không.</param>
        /// <param name="classId">ID của lớp học cần lấy môn học.</param>
        /// <returns>Danh sách môn học liên kết với lớp học.</returns>
        public async Task<Result<IEnumerable<SubjectDTO>>> GetSubjectsClassAsync(bool isHaveClass, Guid classId)
        {
            try
            {
                var subjects = await _unitOfWork.Subjects.GetSubjectsClassAsync(isHaveClass, classId);
                var result = _mapper.Map<IEnumerable<SubjectDTO>>(subjects);
                return new Result<IEnumerable<SubjectDTO>>
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<SubjectDTO>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        /// <summary>
        /// Lấy danh sách các môn học có liên kết với lịch học cụ thể.
        /// </summary>
        /// <param name="isHaveSchedule">Xác định môn học có lịch học hay không.</param>
        /// <param name="scheduleId">ID của lịch học cần lấy môn học.</param>
        /// <returns>Danh sách môn học có liên kết với lịch học.</returns>
        public async Task<Result<IEnumerable<SubjectDTO>>> GetSubjectsScheduleAsync(bool isHaveSchedule, Guid scheduleId)
        {
            try
            {
                var subjects = await _unitOfWork.Subjects.GetSubjectsScheduleAsync(isHaveSchedule, scheduleId);
                var result = _mapper.Map<IEnumerable<SubjectDTO>>(subjects);
                return new Result<IEnumerable<SubjectDTO>>
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<SubjectDTO>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
