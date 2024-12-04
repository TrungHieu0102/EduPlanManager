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
