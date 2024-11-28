using EduPlanManager.Models.Entities;

namespace EduPlanManager.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML);
        Task SendConfirmationEmail(string email, User user, string confirmationLink);
    }
}
