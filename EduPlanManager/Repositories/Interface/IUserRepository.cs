﻿using EduPlanManager.Models.Entities;

namespace EduPlanManager.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task<List<User>> GetUsersWithoutClassAsync();

    }

}
