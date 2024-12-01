using AutoMapper;
using EduPlanManager.Models.DTOs.AcademicTerm;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;

namespace EduPlanManager.Services
{
    public class AcademicTermService : IAcademicTermService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;



        public AcademicTermService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<AcademicTermDTO>> CreateAcademicTermsAsync(CreateUpdateAcademicTermDTO createAcademicTerm)
        {
            try
            {
                var existingTerm = await _unitOfWork.AcademicTerms.CheckExists(createAcademicTerm);
                if (existingTerm != null)
                {
                    throw new Exception("Academic term already exists");
                }
                createAcademicTerm.Id = Guid.NewGuid();
                var academicTerm = _mapper.Map<AcademicTerm>(createAcademicTerm);
                await _unitOfWork.AcademicTerms.AddAsync(academicTerm);
                await _unitOfWork.CompleteAsync();
                return new Result<AcademicTermDTO>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<AcademicTermDTO>(academicTerm)
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
                var subject = _unitOfWork.AcademicTerms.GetByIdAsync(id);
                _unitOfWork.AcademicTerms.Delete(subject.Result);
                if (await _unitOfWork.CompleteAsync() == 0)
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
            return await _unitOfWork.AcademicTerms.GetAllAsync();
        }
        public async Task<Result<AcademicTermDTO>> UpdateAcademic(CreateUpdateAcademicTermDTO request)
        {
            try
            {
                var academicTerm = await _unitOfWork.AcademicTerms.GetByIdAsync(request.Id) ?? throw new Exception("Không tìm thấy");
                _mapper.Map(request, academicTerm);
                await _unitOfWork.CompleteAsync();
                return new Result<AcademicTermDTO>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<AcademicTermDTO>(academicTerm)
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
