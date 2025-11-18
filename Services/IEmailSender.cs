using System.Threading;
using System.Threading.Tasks;

namespace RecruitmentSystem.API.Services;

public interface IEmailSender
{
 Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
