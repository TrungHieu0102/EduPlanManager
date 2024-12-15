using AutoMapper;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.DTOs.SubjectSchedule;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;

namespace EduPlanManager.Services
{
    public class SubjectScheduleService : ISubjectScheduleService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork ;
        public SubjectScheduleService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Tạo mới một lịch học cho môn học. 
        /// Phương thức này kiểm tra xem lịch học có trùng lặp với lịch học đã tồn tại hay không.
        /// Nếu không trùng lặp, sẽ tạo mới một đối tượng `SubjectSchedule` và lưu vào cơ sở dữ liệu.
        /// Trả về đối tượng lịch học vừa tạo dưới dạng `SubjectScheduleDTO`.
        /// </summary>
        /// <param name="dto">Thông tin lịch học cần tạo mới</param>
        /// <returns>Đối tượng `Result<SubjectScheduleDTO>` chứa thông tin lịch học vừa tạo hoặc thông báo lỗi</returns>
        public async Task<Result<SubjectScheduleDTO>> CreateScheduleAsync(CreateSubjectScheduleDTO dto)
        {
            try
            {
                var isDuplicate = await _unitOfWork.SubjectSchedules.IsDuplicateScheduleAsync(dto.DayOfWeek, dto.Session);
                if (isDuplicate)
                {
                    throw new InvalidOperationException("Lịch học này đã tồn tại.");
                }
                var schedule = _mapper.Map<SubjectSchedule>(dto);
                schedule.Id = Guid.NewGuid();
                await _unitOfWork.SubjectSchedules.AddAsync(schedule);
                await _unitOfWork.CompleteAsync();
                var response = _mapper.Map<SubjectScheduleDTO>(schedule);
                return new Result<SubjectScheduleDTO>
                {
                    IsSuccess = true,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new Result<SubjectScheduleDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        /// <summary>
        /// Cập nhật lịch học đã tồn tại. 
        /// Phương thức này kiểm tra xem lịch học có trùng lặp với lịch học đã tồn tại hay không.
        /// Nếu không, sẽ cập nhật lịch học trong cơ sở dữ liệu.
        /// Trả về thông tin lịch học đã cập nhật dưới dạng `SubjectScheduleDTO`.
        /// </summary>
        /// <param name="dto">Thông tin lịch học cần cập nhật</param>
        /// <returns>Đối tượng `Result<SubjectScheduleDTO>` chứa thông tin lịch học đã cập nhật hoặc thông báo lỗi</returns>
        public async Task<Result<SubjectScheduleDTO>> UpdateScheduleAsync(UpdateSubjectScheduleDTO dto)
        {
            try
            {
                var schedule = await _unitOfWork.SubjectSchedules.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException("Lịch học không tồn tại.");
                var isDuplicate = await _unitOfWork.SubjectSchedules.IsDuplicateScheduleAsync(dto.DayOfWeek, dto.Session);
                if (isDuplicate)
                {
                    throw new InvalidOperationException("Lịch học này đã tồn tại.");
                }
                _mapper.Map(dto, schedule);
                await _unitOfWork.SubjectSchedules.UpdateAsync(schedule);
                var response = _mapper.Map<SubjectScheduleDTO>(schedule);
                return new Result<SubjectScheduleDTO>
                {
                    IsSuccess = true,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new Result<SubjectScheduleDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        /// <summary>
        /// Xóa một lịch học theo ID.
        /// Phương thức này sẽ xóa lịch học khỏi cơ sở dữ liệu.
        /// </summary>
        /// <param name="id">ID của lịch học cần xóa</param>
        /// <returns>Đối tượng `Result<bool>` với kết quả thành công hoặc thông báo lỗi</returns>
        public async Task<Result<bool>> DeleteScheduleAsync(Guid id)
        {
            try
            {
                var schedule = await _unitOfWork.SubjectSchedules.GetByIdAsync(id);
                await _unitOfWork.SubjectSchedules.DeleteAsync(schedule);
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
        /// Lấy danh sách tất cả các lịch học.
        /// Phương thức này sẽ truy vấn tất cả lịch học từ cơ sở dữ liệu và trả về danh sách các đối tượng `SubjectScheduleDTO`.
        /// </summary>
        /// <returns>Danh sách các đối tượng `Result<IEnumerable<SubjectScheduleDTO>>` chứa các lịch học hoặc thông báo lỗi</returns>
        public async Task<Result<IEnumerable<SubjectScheduleDTO>>> GetAllSchedulesAsync()
        {
            try
            {
                var schedules = await _unitOfWork.SubjectSchedules.GetAllAsync();
                var result = _mapper.Map<IEnumerable<SubjectScheduleDTO>>(schedules);
                return new Result<IEnumerable<SubjectScheduleDTO>>
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<SubjectScheduleDTO>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        /// <summary>
        /// Lấy lịch học theo ID.
        /// Phương thức này sẽ tìm và trả về thông tin lịch học của môn học theo ID.
        /// Nếu không tìm thấy lịch học, sẽ ném ra ngoại lệ.
        /// </summary>
        /// <param name="id">ID của lịch học cần lấy thông tin</param>
        /// <returns>Đối tượng `Result<SubjectScheduleDTO>` chứa thông tin lịch học hoặc thông báo lỗi</returns>
        public async Task<Result<SubjectScheduleDTO>> GetScheduleByIdAsync(Guid id)
        {
            try
            {
                var schedule = await _unitOfWork.SubjectSchedules.GetByIdAsync(id) ?? throw new Exception("Lịch học không tồn tại.");
               
                var respone = _mapper.Map<SubjectScheduleDTO>(schedule);
                return new Result<SubjectScheduleDTO>
                {
                    IsSuccess = true,
                    Data = respone
                };
            }
            catch (Exception ex)
            {
                return new Result<SubjectScheduleDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        /// <summary>
        /// Thêm các môn học vào lịch học. 
        /// Phương thức này sẽ tìm lịch học theo ID và thêm các môn học vào lịch học này.
        /// </summary>
        /// <param name="subjectIds">Danh sách ID các môn học</param>
        /// <param name="scheduleId">ID của lịch học</param>
        /// <returns>Trả về true nếu thêm thành công, false nếu không tìm thấy lịch học hoặc môn học</returns>
        public async Task<bool> AddSubjectsToSchedule(List<Guid> subjectIds, Guid scheduleId)
        {
            var shedule = await _unitOfWork.SubjectSchedules.GetScheduleSubjectAsync(scheduleId);

            if (shedule == null)
                return false;

            var subjects = await _unitOfWork.Subjects.GetSubjectsByIdsAsync(subjectIds);

            if (!subjects.Any())
                return false;
            foreach (var subject in subjects)
            {
                shedule.Subjects.Add(subject);
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
