using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace RecruitmentSystem.API.Services;

public class SmtpOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string FromName { get; set; } = "Recruitment System";
    public int TimeoutMs { get; set; } = 10000; //10s default
}

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;

    public SmtpEmailSender(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(_options.User, _options.Password),
            Timeout = _options.TimeoutMs
        };
        using var message = new MailMessage
        {
            From = new MailAddress(string.IsNullOrWhiteSpace(_options.From) ? _options.User : _options.From, _options.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(to);
        await client.SendMailAsync(message, cancellationToken);
    }
}
