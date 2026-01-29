using System.Threading.Tasks;

namespace Nafes.API.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}
