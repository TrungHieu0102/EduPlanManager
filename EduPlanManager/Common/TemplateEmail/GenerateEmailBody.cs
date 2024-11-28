using System.Text;

namespace EduPlanManager.Common.TemplateEmail
{
    public static class GenerateEmailBody
    {
       
        public static string GetEmailOTPBody(string userName, string Password)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Common", "TemplateEmail", "PasswordTemplate.html");
            string body = File.ReadAllText(templatePath);
            body = body.Replace("{{UserName}}", userName);
            body = body.Replace("{{Password}}", Password);
            return body;
        }

        public static string GetEmailConfirmationBody(string Email, string url)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Common", "TemplateEmail", "ConfirmLink.html");
            string body = File.ReadAllText(templatePath);
            body = body.Replace("{{Email}}", Email);
            body = body.Replace("{{Link}}", url);
            return body;
        }
       
    }
}
