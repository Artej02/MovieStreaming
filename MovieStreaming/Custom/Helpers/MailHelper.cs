using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using System.Threading.Tasks;
using System;
using MovieStreaming.Custom.DatabaseHelpers;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;
using System.Linq;
using MailKit;
using MovieStreaming.Areas.Admin.Models.User;

namespace MovieStreaming.Custom.Helpers
{
    public class MailHelper
    {
        private IConfiguration Configuration;
        private HttpContext Context;

        public MailHelper(IConfiguration _Configuration)
        {
            Configuration = _Configuration;
        }
        public async Task<object> SendResetPasswordMailAsync(String token, String Username)
        {

            var user = (await new Query().SelectSingle<User>("SELECT * FROM [Users] WHERE Username = @Username", new { @Username = Username }));

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(Configuration["EmailCredenciale:Email"]);
            email.To.Add(MailboxAddress.Parse(user.Result.Username));
            email.Subject = "UBOResearchTool";
            email.Body = new TextPart(TextFormat.Html)
            {

                Text = "<p>Click in this link to reset password: </p><p><a href=" + Configuration["RestPwURL"] + "?resetToken=" + token + ">RESET PASSWORD!</a></p>"
            };

            // send email
            using var smtp = new SmtpClient();
            try
            {
                smtp.CheckCertificateRevocation = false;
                smtp.Connect(Configuration["EmailCredenciale:Smtp"], Int32.Parse(Configuration["EmailCredenciale:Port"]), SecureSocketOptions.StartTls);
                smtp.Authenticate(Configuration["EmailCredenciale:Email"], Configuration["EmailCredenciale:Password"]);
                smtp.Send(email);
                smtp.Disconnect(true);

                return new { Sent = true };

            }
            catch (Exception e)
            {
                String message = e.Message;
                return new { Sent = false, Message = message };
            }

        }

        public async Task<object> SendEmailForTicketCreation(int? userId)
        {
            var userID = (await new Query().SelectSingle<User>("SELECT * FROM [Users] WHERE Id = @Id", new { @Id = userId }));
            var admins = (await new Query().Select<User>("SELECT * FROM [Users] WHERE RoleId = 12")).Result.ToList();

            foreach (var admin in admins)
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(Configuration["EmailCredenciale:Email"]);
                email.To.Add(MailboxAddress.Parse(admin.Username));
                email.Subject = "UBOResearchTool";
                email.Body = new TextPart(TextFormat.Html)
                {


                    Text = $"<p>{userID.Result.Name} has opened a new ticket!</p>"
                };

                // send email
                using var smtp = new SmtpClient();
                try
                {
                    smtp.CheckCertificateRevocation = false;
                    smtp.Connect(Configuration["EmailCredenciale:Smtp"], Int32.Parse(Configuration["EmailCredenciale:Port"]), SecureSocketOptions.StartTls);
                    smtp.Authenticate(Configuration["EmailCredenciale:Email"], Configuration["EmailCredenciale:Password"]);
                    smtp.Send(email);
                    smtp.Disconnect(true);

                }
                catch (Exception e)
                {
                    String message = e.Message;
                    return new { Sent = false, Message = message };
                }
            }
            return new { Sent = true };
        }

    }
}
