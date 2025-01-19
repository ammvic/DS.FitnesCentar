using FitnessCentar.Email.Persistence;
using System.Net.Mail;
using System.Net;

namespace FitnessCentar.Email.Services
{
    public class EmailService
    {
        private readonly EmailDbContext _dbContext;

        public EmailService(EmailDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> SendEmailAsync(string recipient, string subject, string body)
        {
            try
            {
                // Logika za slanje e-maila (SMTP)
                using var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("a.mmvic02@gmail.com", "jzwhpuvvhnmcryyk"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("a.mmvic02@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(recipient);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Email uspešno poslat na: {recipient}");

                // Evidentiranje uspešnog slanja u Neo4j
                await _dbContext.ExecuteWriteAsync(
                    "CREATE (e:EmailLog {Recipient: $recipient, Subject: $subject, Body: $body, SentAt: $sentAt, IsSuccessful: true, ErrorMessage: null})",
                    new { recipient, subject, body, sentAt = DateTime.UtcNow }
                );

                return true;
            }
            catch (Exception ex)
            {
                // Logovanje greške
                Console.WriteLine($"Greška prilikom slanja emaila: {ex.Message}");
                return false;
            }
        }
    }
}
