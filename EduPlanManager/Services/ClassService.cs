using AutoMapper;
using EduPlanManager.Models.DTOs.Class;
using EduPlanManager.Models.DTOs.Respone;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services.Interface;
using EduPlanManager.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace EduPlanManager.Services
{
    public class ClassService : IClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        // Constructor để khởi tạo các dependency cần thiết: unitOfWork, mapper và userManager.
        public ClassService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Tìm kiếm các lớp học với từ khóa tìm kiếm và phân trang.
        /// - Nếu từ khóa tìm kiếm không rỗng, lọc danh sách lớp học có tên hoặc mã lớp bắt đầu với từ khóa tìm kiếm.
        /// - Trả về kết quả với danh sách lớp học đã tìm, tổng số lớp học, tổng số trang và trang hiện tại.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm lớp học.</param>
        /// <param name="pageNumber">Số trang hiện tại.</param>
        /// <param name="pageSize">Số lượng lớp học mỗi trang.</param>
        /// <returns>Kết quả chứa danh sách lớp học và thông tin phân trang.</returns>
        public async Task<ResultPage<ClassDTO>> SearchClassesAsync(string searchTerm, int pageNumber, int pageSize)
        {
            try
            {
                var query = _unitOfWork.Classes.GetQueryable();

                // Nếu có từ khóa tìm kiếm, lọc theo tên lớp hoặc mã lớp.
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(s => s.ClassName.StartsWith(searchTerm) || s.Code.StartsWith(searchTerm));
                }

                // Lấy tổng số lớp học và tính tổng số trang.
                int totalClasses = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalClasses / pageSize);

                // Lấy lớp học theo phân trang.
                var classes = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Áp dụng ánh xạ dữ liệu sang DTO.
                var classDtos = _mapper.Map<IEnumerable<ClassDTO>>(classes);

                return new ResultPage<ClassDTO>
                {
                    IsSuccess = true,
                    Data = classDtos,
                    TotalCount = totalClasses,
                    TotalPages = totalPages,
                    CurrentPage = pageNumber
                };
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

        /// <summary>
        /// Lấy chi tiết thông tin lớp học theo ID.
        /// - Truy vấn lớp học từ cơ sở dữ liệu theo ID.
        /// - Ánh xạ dữ liệu lớp học thành DTO và trả về.
        /// </summary>
        /// <param name="id">ID lớp học cần lấy.</param>
        /// <returns>Kết quả trả về chứa thông tin lớp học.</returns>
        public async Task<Result<CreateUpdateClassDTO>> GetClass(Guid id)
        {
            try
            {
                // Lấy lớp học từ cơ sở dữ liệu.
                var classEntity = await _unitOfWork.Classes.GetByIdAsync(id);
                var response = _mapper.Map<CreateUpdateClassDTO>(classEntity);

                return new Result<CreateUpdateClassDTO>
                {
                    IsSuccess = true,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new Result<CreateUpdateClassDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Đếm số lượng lớp học dựa trên từ khóa tìm kiếm.
        /// - Truy vấn cơ sở dữ liệu để đếm số lượng lớp học thỏa mãn điều kiện tìm kiếm.
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm lớp học.</param>
        /// <returns>Số lượng lớp học thỏa mãn điều kiện tìm kiếm.</returns>
        public async Task<int> CountClassesAsync(string searchTerm)
        {
            return await _unitOfWork.Classes.GetTotalClassesAsync(searchTerm);
        }

        /// <summary>
        /// Tìm kiếm lớp học theo tên hoặc mã lớp.
        /// - Truy vấn các lớp học mà tên hoặc mã lớp bắt đầu với từ khóa tìm kiếm.
        /// - Trả về danh sách lớp học phù hợp dưới dạng DTO.
        /// </summary>
        /// <param name="term">Từ khóa tìm kiếm tên hoặc mã lớp.</param>
        /// <returns>Kết quả chứa danh sách lớp học tìm được.</returns>
        public async Task<Result<IEnumerable<ClassDTO>>> SearchClassesByNameOrCodeAsync(string term)
        {
            try
            {
                var classList = await _unitOfWork.Classes
                    .FindAsync(s => s.ClassName.StartsWith(term) || s.Code.StartsWith(term))
                    .Take(10)
                    .ToListAsync();

                var classDto = _mapper.Map<IEnumerable<ClassDTO>>(classList);

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

        /// <summary>
        /// Xóa lớp học theo ID.
        /// - Tìm lớp học cần xóa theo ID.
        /// - Xóa lớp học và lưu thay đổi vào cơ sở dữ liệu.
        /// - Nếu không có thay đổi nào được lưu, ném ra ngoại lệ.
        /// </summary>
        /// <param name="id">ID lớp học cần xóa.</param>
        /// <returns>Kết quả trả về xác nhận xóa thành công.</returns>
        public async Task<Result<bool>> DeleteClassById(Guid id)
        {
            try
            {
                var classEntity = await _unitOfWork.Classes.GetByIdAsync(id);
                _unitOfWork.Classes.Delete(classEntity);

                // Kiểm tra xem có thay đổi nào không.
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

        /// <summary>
        /// Cập nhật thông tin lớp học.
        /// - Kiểm tra lớp học có tồn tại hay không.
        /// - Kiểm tra xem tên lớp hoặc mã lớp có trùng với lớp khác không.
        /// - Áp dụng dữ liệu cập nhật và lưu vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="classRequest">DTO chứa thông tin lớp học cần cập nhật.</param>
        /// <returns>Kết quả trả về với thông tin lớp học đã cập nhật.</returns>
        public async Task<Result<ClassDTO>> UpdateClass(CreateUpdateClassDTO classRequest)
        {
            try
            {
                // Kiểm tra lớp học có tồn tại không.
                var existingClass = await _unitOfWork.Classes.GetByIdAsync(classRequest.Id) ?? throw new Exception("Không tìm thấy lớp học");

                // Kiểm tra tên lớp hoặc mã lớp có tồn tại không.
                if (!await _unitOfWork.Classes.CheckExists(classRequest.ClassName, classRequest.Code))
                {
                    throw new Exception("Tên lớp hoặc mã lớp đã tồn tại");
                }

                // Áp dụng các thay đổi từ DTO vào đối tượng lớp học hiện tại.
                _mapper.Map(classRequest, existingClass);
                _unitOfWork.Classes.Update(existingClass);

                var result = await _unitOfWork.CompleteAsync();
                var response = _mapper.Map<ClassDTO>(existingClass);

                return new Result<ClassDTO>
                {
                    IsSuccess = true,
                    Data = response
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

        /// <summary>
        /// Xóa nhiều lớp học theo danh sách ID.
        /// - Lấy danh sách lớp học cần xóa theo các ID.
        /// - Xóa các lớp học và lưu thay đổi vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="ids">Danh sách các ID lớp học cần xóa.</param>
        public async Task DeleteClassesAsync(List<Guid> ids)
        {
            var classes = await _unitOfWork.Classes.GetClassesByIdsAsync(ids);
            if (classes.Count != 0)
            {
                await _unitOfWork.Classes.DeleteClassesAsync(classes);
            }
        }

        /// <summary>
        /// Tạo mới một lớp học.
        /// - Kiểm tra xem lớp học có tồn tại không.
        /// - Nếu không tồn tại, tạo mới lớp học và lưu vào cơ sở dữ liệu.
        /// - Trả về thông tin lớp học đã tạo.
        /// </summary>
        /// <param name="classRequest">DTO chứa thông tin lớp học cần tạo mới.</param>
        /// <returns>Kết quả trả về với thông tin lớp học đã tạo mới.</returns>
        public async Task<Result<ClassDTO>> CreateClassAsync(CreateUpdateClassDTO classRequest)
        {
            try
            {
                // Kiểm tra xem lớp học đã tồn tại hay chưa.
                if (!await _unitOfWork.Classes.CheckExists(classRequest.ClassName, classRequest.Code))
                {
                    throw new Exception("Lớp học này đã tồn tại");
                }

                // Tạo lớp học mới với ID ngẫu nhiên.
                classRequest.Id = Guid.NewGuid();
                var classEntity = _mapper.Map<Class>(classRequest);
                classEntity.Code = classEntity.Code.ToUpper();

                // Thêm lớp học vào cơ sở dữ liệu.
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

        /// <summary>
        /// Lấy chi tiết lớp học theo ID.
        /// - Truy vấn lớp học từ cơ sở dữ liệu theo ID.
        /// - Trả về thông tin lớp học dưới dạng DTO.
        /// </summary>
        /// <param name="id">ID lớp học cần lấy thông tin chi tiết.</param>
        /// <returns>Kết quả trả về chứa thông tin chi tiết của lớp học.</returns>
        public async Task<Result<ClassDTO>> GetClassDetailAsync(Guid id)
        {
            try
            {
                var classEntity = await _unitOfWork.Classes.GetByIdAsync(id) ?? throw new Exception("Không tìm thấy lớp học");
                var response = _mapper.Map<ClassDTO>(classEntity);

                return new Result<ClassDTO>
                {
                    IsSuccess = true,
                    Data = response
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

        /// <summary>
        /// Thêm người dùng vào lớp học.
        /// - Lấy lớp học từ cơ sở dữ liệu theo ID và kiểm tra người dùng có thuộc lớp không.
        /// - Cập nhật số lượng sinh viên và giảng viên trong lớp học nếu có sự thay đổi.
        /// </summary>
        /// <param name="userIds">Danh sách các ID người dùng cần thêm vào lớp.</param>
        /// <param name="classId">ID lớp học cần thêm người dùng vào.</param>
        /// <returns>Trả về true nếu việc thêm người dùng thành công, false nếu không.</returns>
        public async Task<bool> AddUsersToClass(List<Guid> userIds, Guid classId)
        {
            var classEntity = await _unitOfWork.Classes.GetClassUserAsync(classId);

            if (classEntity == null)
                return false;

            var users = await _unitOfWork.Classes.GetUsersByIdsAsync(userIds);

            if (!users.Any())
                return false;

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (!classEntity.Users.Contains(user))
                {
                    classEntity.Users.Add(user);

                    // Cập nhật số lượng sinh viên và giảng viên trong lớp học.
                    if (roles.Contains("Student"))
                        classEntity.StudentCount++;
                    else if (roles.Contains("Teacher"))
                        classEntity.TeacherCount++;
                }
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }

        /// <summary>
        /// Thêm môn học vào lớp học.
        /// - Lấy lớp học theo ID và kiểm tra môn học có tồn tại không.
        /// - Cập nhật danh sách môn học trong lớp.
        /// </summary>
        /// <param name="subjectIds">Danh sách các ID môn học cần thêm vào lớp.</param>
        /// <param name="classId">ID lớp học cần thêm môn học vào.</param>
        /// <returns>Trả về true nếu việc thêm môn học thành công, false nếu không.</returns>
        public async Task<bool> AddSubjectsToClass(List<Guid> subjectIds, Guid classId)
        {
            var classEntity = await _unitOfWork.Classes.GetClassSubjectAsync(classId);

            if (classEntity == null)
                return false;

            var subjects = await _unitOfWork.Subjects.GetSubjectsByIdsAsync(subjectIds);

            if (!subjects.Any())
                return false;

            foreach (var subject in subjects)
            {
                classEntity.Subjects.Add(subject);
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
