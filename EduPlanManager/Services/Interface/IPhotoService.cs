using CloudinaryDotNet.Actions;

namespace EduPlanManager.Services.Interface
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file, string folder);
        Task<DeletionResult> DeletePhotoAsync(string folder, string publicId);
    }
}
