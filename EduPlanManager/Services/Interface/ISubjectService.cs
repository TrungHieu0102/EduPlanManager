using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.DTOs.Subject;
using EduPlanManager.Models.Entities;

namespace EduPlanManager.Services.Interface
{
    public interface ISubjectService
    {
        Task<ResultPage<SubjectDTO>> SearchSubjectsAsync(string searchTerm, int? semester, int? year, int pageNumber, int pageSize);
        Task<int> CountSubjectsAsync(string searchTerm, int? semester, int? year);
        Task<Result<IEnumerable<SubjectDTO>>> SearchSubjectsByNameOrCodeAsync(string term);
        Task<Result<SubjectDetailDTO>> GetSubjectWithDetailsAsync(Guid id);
        Task<Result<bool>> DeleteSubjectById(Guid id);
        Task<Result<Subject>> UpdateSubject(Subject subject);
        Task<Result<Subject>> GetSubject(Guid id);
        Task DeleteSubjectsAsync(List<Guid> ids);
        Task<Result<SubjectDTO>> CreateSubjectAsync(SubjectCreateDTO subjectCreateDTO);

    }
}
