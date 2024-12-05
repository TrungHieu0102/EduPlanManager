using AutoMapper;
using EduPlanManager.Models.DTOs.AcademicTerm;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;
namespace EduPlanManager.Services
{
    public class AcademicTermService(IUnitOfWork unitOfWork, IMapper mapper) : IAcademicTermService
    {
        public async Task<Result<AcademicTermDTO>> CreateAcademicTermsAsync(CreateUpdateAcademicTermDTO createAcademicTerm)
        {
            try
            {
                var existingTerm = await unitOfWork.AcademicTerms.CheckExists(createAcademicTerm);
                if (existingTerm != null)
                {
                    throw new Exception("Academic term already exists");
                }
                createAcademicTerm.Id = Guid.NewGuid();
                var academicTerm = mapper.Map<AcademicTerm>(createAcademicTerm);
                await unitOfWork.AcademicTerms.AddAsync(academicTerm);
                await unitOfWork.CompleteAsync();
                return new Result<AcademicTermDTO>
                {
                    IsSuccess = true,
                    Data = mapper.Map<AcademicTermDTO>(academicTerm)
                };
            }
            catch (Exception ex)
            {
                return new Result<AcademicTermDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<Result<bool>> DeleteAcademicAsync(Guid id)
        {
            try
            {
                var subject = unitOfWork.AcademicTerms.GetByIdAsync(id);
                unitOfWork.AcademicTerms.Delete(subject.Result);
                if (await unitOfWork.CompleteAsync() == 0)
                {
                    throw new Exception("Delete failed");
                }
                return new Result<bool>
                {
                    IsSuccess = true
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

        public async Task<IEnumerable<AcademicTerm>> GetAllAcademicTermsAsync()
        {
            return await unitOfWork.AcademicTerms.GetAllAsync();
        }
        public async Task<Result<AcademicTermDTO>> UpdateAcademic(CreateUpdateAcademicTermDTO request)
        {
            try
            {
                var academicTerm = await unitOfWork.AcademicTerms.GetByIdAsync(request.Id) ?? throw new Exception("Không tìm thấy");
                mapper.Map(request, academicTerm);
                await unitOfWork.CompleteAsync();
                return new Result<AcademicTermDTO>
                {
                    IsSuccess = true,
                    Data = mapper.Map<AcademicTermDTO>(academicTerm)
                };
            }
            catch (Exception ex)
            {
                return new Result<AcademicTermDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
