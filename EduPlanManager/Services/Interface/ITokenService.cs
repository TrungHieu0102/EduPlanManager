using EduPlanManager.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace EduPlanManager.Services.Interface
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        bool IsAccessTokenExpired(string accessToken);
        Task<string> RefreshAccessTokenAsync(string refreshToken);
    }
}
