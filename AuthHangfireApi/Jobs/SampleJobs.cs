using System.Net.Mail;
using System.Net;
using Hangfire;
using AuthHangfireApi.Models;
using AuthHangfireApi.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthHangfireApi.Jobs
{
    public class SampleJobs
    {
        private readonly EmailSettings _emailSettings;

        public SampleJobs(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public void SendEmail(string to, string message)
        {
            try
            {
                using var smtp = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = _emailSettings.UseTLS
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.Username, _emailSettings.SenderName),
                    Subject = "Test Email from Hangfire",
                    Body = message,
                    IsBodyHtml = false
                };

                mailMessage.To.Add(to);

                smtp.Send(mailMessage);

                Console.WriteLine($"[Email Job] Sent email to {to}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Job] Failed to send email: {ex.Message}");
            }
        }

        [AutomaticRetry(Attempts = 3)]
        public void CleanLogs()
        {
            Console.WriteLine($"🧹 Logs cleaned at {DateTime.Now}");
        }
    }
}
