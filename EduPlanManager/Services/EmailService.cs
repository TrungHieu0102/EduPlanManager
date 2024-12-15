using EduPlanManager.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using EduPlanManager.Services.Interface;
using EduPlanManager.Common.TemplateEmail;

namespace EduPlanManager.Services
{
    public class EmailService(IConfiguration config, UserManager<User> userManager) : IEmailService
    {
        /// <summary>
        /// Gửi email bất đồng bộ.
        /// - Phương thức này gửi email đến địa chỉ `toEmail` với chủ đề `subject` và nội dung `body`.
        /// - Cài đặt email được lấy từ cấu hình `EmailSettings` trong file cấu hình ứng dụng.
        /// - Cấu hình máy chủ email và cổng máy chủ được xác định trong `config`.
        /// - Đối tượng `SmtpClient` dùng để thiết lập kết nối với máy chủ SMTP và gửi email.
        /// </summary>
        /// <param name="toEmail">Địa chỉ email người nhận.</param>
        /// <param name="subject">Chủ đề của email.</param>
        /// <param name="body">Nội dung email.</param>
        /// <param name="isBodyHtml">Xác định liệu nội dung email có phải là HTML hay không.</param>
        /// <returns>Task cho phép gửi email bất đồng bộ.</returns>
        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml)
        {
            // Lấy cấu hình các thông số email từ file cấu hình ứng dụng.
            string mailServer = config["EmailSettings:MailServer"]!;
            string fromEmail = config["EmailSettings:FromEmail"]!;
            string password = config["EmailSettings:Password"]!;
            int port = int.Parse(config["EmailSettings:MailPort"]!);

            // Tạo đối tượng SmtpClient để gửi email qua máy chủ SMTP.
            var client = new SmtpClient(mailServer, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),  // Thiết lập thông tin xác thực.
                EnableSsl = true, // Bật chế độ mã hóa SSL cho kết nối.
            };

            // Tạo đối tượng MailMessage với các thông tin email cần gửi.
            var mailMessage = new MailMessage(fromEmail, toEmail, subject, body)
            {
                IsBodyHtml = isBodyHtml // Đặt cờ xác định liệu nội dung là HTML hay văn bản thuần túy.
            };

            // Gửi email một cách bất đồng bộ.
            return client.SendMailAsync(mailMessage);
        }

        /// <summary>
        /// Gửi email xác nhận yêu cầu cấp lại mật khẩu.
        /// - Phương thức này tạo một liên kết xác nhận mật khẩu mới và gửi email đến người dùng.
        /// - Đầu tiên, hệ thống tạo mã xác nhận email thông qua `userManager.GenerateEmailConfirmationTokenAsync`.
        /// - Sau đó, hệ thống xây dựng nội dung email thông qua phương thức `GenerateEmailBody.GetEmailConfirmationBody`.
        /// - Cuối cùng, phương thức `SendEmailAsync` sẽ gửi email xác nhận đến địa chỉ của người dùng.
        /// </summary>
        /// <param name="email">Địa chỉ email của người nhận (người dùng).</param>
        /// <param name="user">Đối tượng người dùng cần xác nhận.</param>
        /// <param name="confirmationLink">Liên kết xác nhận cho người dùng.</param>
        /// <returns>Task bất đồng bộ khi gửi email thành công.</returns>
        public async Task SendConfirmationEmail(string email, User user, string confirmationLink)
        {
            // Tạo mã xác nhận cho email của người dùng.
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            // Tạo nội dung email xác nhận dựa trên tên người dùng và liên kết xác nhận.
            var body = GenerateEmailBody.GetEmailConfirmationBody(user.GetFullName(), confirmationLink);

            // Gửi email xác nhận với chủ đề "Yêu cầu cấp lại mật khẩu" và nội dung HTML.
            await SendEmailAsync(email, "Yêu cầu cấp lại mật khẩu", body, true);
        }
    }
}
