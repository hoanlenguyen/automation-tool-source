using BITool.Helpers;
using BITool.Models.SendMail;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Reflection;

namespace BITool.Services
{
    public interface ISendMailService
    {
        Task SendMailAsync(SendMailDto input);
    }

    public class SendMailService : ISendMailService
    {
        private readonly string SMTP_HOST;
        private readonly int SMTP_PORT;
        private readonly string SMTP_USERNAME;
        private readonly string SMTP_PASSWORD;
        private readonly string EMAIL_SENDER_NAME;
        private readonly string EMAIL_FROM;
        private readonly bool EMAIL_IS_SEND_SPECIFIC_MAIL;
        private readonly string EMAIL_TEST_MAIL_ADDRESS;
        private readonly string EMAIL_CC;
        private readonly string EMAIL_BCC;

        public SendMailService(
            IConfiguration configuration
            )
        {
            SMTP_HOST = configuration["SMTP_HOST"];
            SMTP_PORT = configuration.GetValue<int>("SMTP_PORT");
            SMTP_USERNAME = configuration["SMTP_USERNAME"];
            SMTP_PASSWORD = configuration["SMTP_PASSWORD"];
            EMAIL_SENDER_NAME = configuration["EMAIL_SENDER_NAME"];
            EMAIL_FROM = configuration["EMAIL_FROM"];
            EMAIL_IS_SEND_SPECIFIC_MAIL = configuration.GetValue<bool>("EMAIL_IS_SEND_SPECIFIC_MAIL");
            EMAIL_TEST_MAIL_ADDRESS = configuration["EMAIL_TEST_MAIL_ADDRESS"];
            EMAIL_CC = configuration["EMAIL_CC"];
            EMAIL_BCC = configuration["EMAIL_BCC"];
        }

        public async Task SendMailAsync(SendMailDto input)
        {
            var mailDto = new MimeMessage();
            var sender = new MailboxAddress(EMAIL_SENDER_NAME, EMAIL_FROM);
            mailDto.From.Add(sender);
            input.FromAddress = EMAIL_FROM;
            if (EMAIL_IS_SEND_SPECIFIC_MAIL)
            {
                input.ToAddresses = EMAIL_TEST_MAIL_ADDRESS;
                input.CC = string.Empty;
                input.BCC = string.Empty;
            }
            else
            {
                if (string.IsNullOrEmpty(input.CC))
                    input.CC = EMAIL_CC;

                if (string.IsNullOrEmpty(input.BCC))
                    input.BCC = EMAIL_BCC;
            }
            var multipart = new Multipart("mixed");

            if (input.TemplateName.IsNotNullOrEmpty())
            {
                var mailTemplate = await GetMailTemplate(input.TemplateName);
                if (mailTemplate.IsNotNullOrEmpty())
                {
                    multipart.Add(new TextPart(MimeKit.Text.TextFormat.Html) { Text = await GetCompiledMailBody(input.BodyData, mailTemplate) });
                    mailDto.Subject = /*mailTemplate.Subject??*/ input.Subject;
                }
            }
            if (mailDto.Subject.IsNullOrEmpty())
            {
                multipart.Add(input.HtmlContent.IsNotNullOrEmpty() ? new TextPart(MimeKit.Text.TextFormat.Html) { Text = input.HtmlContent } : new TextPart(MimeKit.Text.TextFormat.Text) { Text = input.TextContent });
                mailDto.Subject = input.Subject;
            }
            mailDto.Body = multipart;
            mailDto.Bcc.Clear();
            mailDto.Cc.Clear();

            if (input.CC.IsNotNullOrEmpty())
                mailDto.Cc.AddRange(input.CC.ToMailAddresses());
            if (input.BCC.IsNotNullOrEmpty())
                mailDto.Bcc.AddRange(input.BCC.ToMailAddresses());

            if (input.IsSendInSeparateEmail)
            {
                var toAddresses = input.ToAddresses.ToMailAddresses();
                foreach (var item in toAddresses)
                {
                    mailDto.To.Add(item);
                    await Send(mailDto);
                }
            }
            else
            {
                mailDto.To.AddRange(input.ToAddresses.ToMailAddresses());
                await Send(mailDto);
            }
        }

        private async Task Send(MimeMessage message)
        {
            using (var smtpClient = new SmtpClient())
            {
                //Console.WriteLine($"{SMTP_PORT}-{SMTP_USERNAME}-{SMTP_PASSWORD}");
                smtpClient.Connect(SMTP_HOST, SMTP_PORT, SecureSocketOptions.StartTls);
                smtpClient.Authenticate(SMTP_USERNAME, SMTP_PASSWORD);
                await smtpClient.SendAsync(message);
                smtpClient.Disconnect(true);
            }
        }

        private async Task BulkSend(List<MimeMessage> messages)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(SMTP_HOST, SMTP_PORT, SecureSocketOptions.StartTls);
                smtpClient.Authenticate(SMTP_USERNAME, SMTP_PASSWORD);
                for (int i = 0; i < messages.Count; i++)
                {
                    await smtpClient.SendAsync(messages[i]);
                }
                smtpClient.Disconnect(true);
            }
        }

        private async Task<string> GetMailTemplate(string mailTemplateCode)
        {
            return string.Empty;
        }

        private async Task<string> GetMailTemplateLayoutBody()
        {
            return string.Empty;
        }

        private async Task<string> GetCompiledMailBody(/*MailBodyParamsDto*/ object input, string mailBodyTemplate, bool requireLayout = true)
        {
            var mailBody = ParseMailBody(mailBodyTemplate, input);
            if (!requireLayout)
            {
                return mailBody;
            }
            var mailLayoutContent = await GetMailTemplateLayoutBody();
            var mailTemplate = mailLayoutContent.Replace("@RenderBody()", mailBody).Replace("@@", "@");
            return mailTemplate;
        }

        private string ParseMailBody(string body, object input)
        {
            if (input == null)
                return body;

            foreach (PropertyInfo item in input.GetType().GetProperties())
            {
                body = body.Replace($"@Model.{item.Name}", item.GetValue(input)?.ToString());
            }
            return body;
        }
    }
}