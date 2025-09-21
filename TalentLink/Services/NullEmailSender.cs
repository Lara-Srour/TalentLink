using Microsoft.AspNetCore.Identity.UI.Services;
namespace TalentLink.Services
{
    public class NullEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Intentionally does nothing
            return Task.CompletedTask;
        }
    }
   
}
