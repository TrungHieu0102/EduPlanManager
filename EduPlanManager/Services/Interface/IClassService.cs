using EduPlanManager.Models.DTOs.Class;
using EduPlanManager.Models.DTOs.Respone;

namespace EduPlanManager.Services.Interface
{
    public interface IClassService
    {
        Task<ResultPage<ClassDTO>> SearchClassesAsync(string searchTerm, int pageNumber, int pageSize);
        Task<Result<ClassDTO>> GetClass(Guid id);
        Task<int> CountClassesAsync(string searchTerm);
        Task<Result<IEnumerable<ClassDTO>>> SearchClassesByNameOrCodeAsync(string term);
        Task<Result<bool>> DeleteClassById(Guid id);
        Task<Result<ClassDTO>> UpdateClass(CreateUpdateClassDTO classRequest);
        Task DeleteClassesAsync(List<Guid> ids);
        Task<Result<ClassDTO>> CreateClassAsync(CreateUpdateClassDTO classRequest);
    }
}
