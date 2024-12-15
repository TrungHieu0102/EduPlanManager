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
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public AcademicTermService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <summary>
        /// Tạo mới một học kỳ.
        /// - Kiểm tra xem học kỳ đã tồn tại trong hệ thống hay chưa. Nếu đã tồn tại, ném ra ngoại lệ.
        /// - Nếu chưa tồn tại, tạo mới một học kỳ với ID được sinh ngẫu nhiên và lưu vào cơ sở dữ liệu.
        /// - Sau khi tạo thành công, trả về kết quả thành công cùng thông tin học kỳ đã tạo.
        /// </summary>
        /// <param name="createAcademicTerm">DTO chứa dữ liệu của học kỳ cần tạo mới.</param>
        /// <returns>Kết quả trả về chứa DTO của học kỳ đã tạo.</returns>
        public async Task<Result<AcademicTermDTO>> CreateAcademicTermsAsync(CreateUpdateAcademicTermDTO createAcademicTerm)
        {
            try
            {
                // Kiểm tra xem học kỳ có tồn tại hay không
                var existingTerm = await unitOfWork.AcademicTerms.CheckExists(createAcademicTerm);
                if (existingTerm != null)
                {
                    throw new Exception("Academic term already exists"); // Nếu đã tồn tại, thông báo lỗi.
                }

                // Tạo mới học kỳ với ID ngẫu nhiên
                createAcademicTerm.Id = Guid.NewGuid();
                var academicTerm = mapper.Map<AcademicTerm>(createAcademicTerm);

                // Thêm học kỳ vào cơ sở dữ liệu
                await unitOfWork.AcademicTerms.AddAsync(academicTerm);
                await unitOfWork.CompleteAsync(); // Lưu thay đổi vào cơ sở dữ liệu

                // Trả về kết quả thành công cùng thông tin học kỳ đã tạo
                return new Result<AcademicTermDTO>
                {
                    IsSuccess = true,
                    Data = mapper.Map<AcademicTermDTO>(academicTerm)
                };
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra, trả về kết quả thất bại với thông báo lỗi
                return new Result<AcademicTermDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Xóa một học kỳ theo ID.
        /// - Lấy thông tin học kỳ cần xóa từ cơ sở dữ liệu dựa trên ID.
        /// - Thực hiện xóa học kỳ đó và kiểm tra kết quả.
        /// - Nếu xóa thành công, trả về kết quả thành công.
        /// - Nếu xóa thất bại (số dòng ảnh hưởng bằng 0), ném ra ngoại lệ và trả về kết quả thất bại.
        /// </summary>
        /// <param name="id">ID của học kỳ cần xóa.</param>
        /// <returns>Kết quả trả về cho biết xóa thành công hay thất bại.</returns>
        public async Task<Result<bool>> DeleteAcademicAsync(Guid id)
        {
            try
            {
                // Lấy học kỳ từ cơ sở dữ liệu
                var subject = await unitOfWork.AcademicTerms.GetByIdAsync(id);
                unitOfWork.AcademicTerms.Delete(subject); // Thực hiện xóa học kỳ

                // Kiểm tra xem có thay đổi nào xảy ra không (số dòng bị ảnh hưởng)
                if (await unitOfWork.CompleteAsync() == 0)
                {
                    throw new Exception("Delete failed"); // Nếu không có thay đổi, ném ra ngoại lệ.
                }

                return new Result<bool>
                {
                    IsSuccess = true // Xóa thành công
                };
            }
            catch (Exception ex)
            {
                return new Result<bool>
                {
                    IsSuccess = false, // Xóa thất bại
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Lấy tất cả các học kỳ từ cơ sở dữ liệu.
        /// - Truy vấn cơ sở dữ liệu để lấy tất cả các học kỳ.
        /// - Trả về danh sách các học kỳ.
        /// </summary>
        /// <returns>Danh sách tất cả học kỳ.</returns>
        public async Task<IEnumerable<AcademicTerm>> GetAllAcademicTermsAsync()
        {
            // Trả về tất cả học kỳ từ cơ sở dữ liệu
            return await unitOfWork.AcademicTerms.GetAllAsync();
        }

        /// <summary>
        /// Cập nhật thông tin học kỳ.
        /// - Lấy học kỳ từ cơ sở dữ liệu theo ID.
        /// - Nếu không tìm thấy học kỳ, ném ra ngoại lệ.
        /// - Áp dụng dữ liệu từ DTO vào học kỳ và lưu lại vào cơ sở dữ liệu.
        /// - Sau khi cập nhật thành công, trả về kết quả thành công cùng thông tin học kỳ đã cập nhật.
        /// </summary>
        /// <param name="request">DTO chứa thông tin học kỳ cần cập nhật.</param>
        /// <returns>Kết quả trả về chứa DTO của học kỳ đã cập nhật.</returns>
        public async Task<Result<AcademicTermDTO>> UpdateAcademic(CreateUpdateAcademicTermDTO request)
        {
            try
            {
                // Lấy học kỳ từ cơ sở dữ liệu theo ID
                var academicTerm = await unitOfWork.AcademicTerms.GetByIdAsync(request.Id) ?? throw new Exception("Không tìm thấy");

                // Áp dụng dữ liệu từ DTO vào học kỳ
                mapper.Map(request, academicTerm);
                await unitOfWork.CompleteAsync(); // Lưu thay đổi vào cơ sở dữ liệu

                // Trả về kết quả thành công với học kỳ đã cập nhật
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
                    IsSuccess = false, // Nếu có lỗi, trả về kết quả thất bại
                    Message = ex.Message
                };
            }
        }
    }
}
