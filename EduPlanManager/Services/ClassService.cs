using AutoMapper;
using EduPlanManager.Models.DTOs.Class;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace EduPlanManager.Services
{
    public class ClassService : IClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ClassService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ResultPage<ClassDTO>> SearchClassesAsync(string searchTerm, int pageNumber, int pageSize)
        {
            try
            {
                var query = _unitOfWork.Classes.GetQueryable();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(s => s.ClassName.StartsWith(searchTerm) || s.Code.StartsWith(searchTerm));
                }


                int totalClasses = await query.CountAsync();

                var totalPages = (int)Math.Ceiling((double)totalClasses / pageSize);

                var subjects = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var subjectDto = _mapper.Map<IEnumerable<ClassDTO>>(subjects);

                var result = new ResultPage<ClassDTO>
                {
                    IsSuccess = true,
                    Data = subjectDto,
                    TotalCount = totalClasses,
                    TotalPages = totalPages,
                    CurrentPage = pageNumber
                };

                return result;
            }
            catch (Exception ex)
            {
                return new ResultPage<ClassDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Result<ClassDTO>> GetClass(Guid id)
        {
            try
            {
                var classEntity = await _unitOfWork.Classes.GetByIdAsync(id);
                var respone = _mapper.Map<ClassDTO>(classEntity);
                return new Result<ClassDTO>
                {
                    IsSuccess = true,
                    Data = respone
                };
            }
            catch (Exception ex)
            {
                return new Result<ClassDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<int> CountClassesAsync(string searchTerm)
        {
            return await _unitOfWork.Classes.GetTotalClassesAsync(searchTerm);
        }
        public async Task<Result<IEnumerable<ClassDTO>>> SearchClassesByNameOrCodeAsync(string term)
        {
            try
            {
                var listClass = await _unitOfWork.Classes
               .FindAsync(s => s.ClassName.StartsWith(term) || s.Code.StartsWith(term))
               .Take(10).ToListAsync();
                var classDto = _mapper.Map<IEnumerable<ClassDTO>>(listClass);
                return new Result<IEnumerable<ClassDTO>>
                {
                    IsSuccess = true,
                    Data = classDto
                };
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<ClassDTO>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }

        }
        public async Task<Result<bool>> DeleteClassById(Guid id)
        {
            try
            {
                var classEntity = await _unitOfWork.Classes.GetByIdAsync(id);
                _unitOfWork.Classes.Delete(classEntity);
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
        public async Task<Result<ClassDTO>> UpdateClass(CreateUpdateClassDTO classRequest)
        {
            try
            {
                var existingClass = await _unitOfWork.Classes.GetByIdAsync(classRequest.Id) ?? throw new Exception("Không tìm thấy môn học");
                if (await _unitOfWork.Classes.CheckExists(classRequest.ClassName, classRequest.Code))
                {
                    throw new Exception("Tên lớp hoặc mã lớp đã tồn tại");
                }
                _mapper.Map(classRequest, existingClass);
                _unitOfWork.Classes.Update(existingClass);
                var result = await _unitOfWork.CompleteAsync();
                var respone = _mapper.Map<ClassDTO>(existingClass);
                return new Result<ClassDTO>
                {
                    IsSuccess = true,
                    Data = respone
                };
            }
            catch (Exception ex)
            {
                return new Result<ClassDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task DeleteClassesAsync(List<Guid> ids)
        {
            var classes = await _unitOfWork.Classes.GetClassesByIdsAsync(ids);
            if (classes.Count != 0)
            {
                await _unitOfWork.Classes.DeleteClassesAsync(classes);
            }
        }
        public async Task<Result<ClassDTO>> CreateClassAsync(CreateUpdateClassDTO classRequest)
        {
            try
            {
                classRequest.Id = Guid.NewGuid();
                var classEntity = _mapper.Map<Class>(classRequest);
                classEntity.Code = classEntity.Code.ToUpper();
                _unitOfWork.Classes.Add(classEntity);
                await _unitOfWork.CompleteAsync();
                var result = _mapper.Map<ClassDTO>(classEntity);
                return new Result<ClassDTO>
                {
                    IsSuccess = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new Result<ClassDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
