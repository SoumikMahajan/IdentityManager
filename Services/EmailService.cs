using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace IdentityManager.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            //var message = new MimeMessage();
            //message.From.Add(new MailboxAddress("Soumik Mahajan", "soumik800@gmail.com"));
            //message.To.Add(new MailboxAddress("", to));
            //message.Subject = subject;

            //var builder = new BodyBuilder
            //{
            //    HtmlBody = body,
            //};

            //message.Body = builder.ToMessageBody();

            var email = new MimeMessage()
            {
                Subject = subject,
                To = { MailboxAddress.Parse(to) },
                Body = new TextPart(TextFormat.Html)
                {
                    Text = body
                },
                From = { MailboxAddress.Parse("soumik800@gmail.com") },
            };


            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("soumik800@gmail.com", "ptyr jhlm xhci shad");
                    await client.SendAsync(email);
                }
                catch (Exception ex)
                {
                    // Handle exceptions here
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
