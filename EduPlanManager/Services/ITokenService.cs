using EduPlanManager.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace EduPlanManager.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
