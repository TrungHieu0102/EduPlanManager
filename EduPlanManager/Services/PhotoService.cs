using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.UnitOfWork;
using EduPlanManager.Services.Interface;

namespace EduPlanManager.Services
{
    public class PhotoService(Cloudinary cloudinary): IPhotoService
    {
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file, string folder)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = folder

                };
                uploadResult = await cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string folder, string publicId)
        {
            var deleteParams = new DeletionParams($"{folder}/{publicId}");
            var result = await cloudinary.DestroyAsync(deleteParams);
            return result;
        }

    }
}

