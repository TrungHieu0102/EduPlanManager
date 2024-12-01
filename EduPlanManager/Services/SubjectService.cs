using AutoMapper;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.DTOs.Subject;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;
using Microsoft.EntityFrameworkCore;

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
                    query = query.Where(s => s.Semester == semester);
                }
                if (year.HasValue)
                {
                    query = query.Where(s => s.Year == year);
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
        public async Task<int> CountSubjectsAsync(string searchTerm, int? semester, int? year)
        {
            return await _unitOfWork.Subjects.GetTotalSubjectsAsync(searchTerm, semester, year);
        }
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

    }
}
