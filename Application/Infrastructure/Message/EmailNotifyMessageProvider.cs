using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Request;
using Application.Response;
using Domain.Configuration;
using eXtensionSharp;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.Infrastructure.Message
{
    public record EmailNotifyMessageRequest(string[] toMails, string subject, string body, IEnumerable<IFormFile> attachments)
        : INotifyMessageRequest;
    
    public class EmailNotifyMessageProvider : NotifyMessageProviderBase
    {
        private readonly IOptions<MailSetting> _options;
        public EmailNotifyMessageProvider(IOptions<MailSetting> options)
        {
            _options = options;
        }
        public override async Task<IResult> SendMessageAsync(INotifyMessageRequest request)
        {
            var emailSettings = _options.Value;
            var mailRequest = request as EmailNotifyMessageRequest;
            if (mailRequest == null) mailRequest = (EmailNotifyMessageRequest)ConvertRequest(request);
            if (mailRequest == null) throw new Exception("Mail request is null. Check request.");
            
            var email = new MimeMessage();
            var sender = new MailboxAddress(emailSettings.DisplayName, emailSettings.FromMail);
            email.Sender = sender;
            mailRequest.toMails.xForEach(item =>
            {
                email.To.Add(MailboxAddress.Parse(item));
            });
            email.Subject = mailRequest.subject;
            var builder = new BodyBuilder();
            if (mailRequest.attachments.xIsNotEmpty())
            {
                byte[] fileBytes;
                mailRequest.attachments.xForEach(item =>
                {
                    if (item.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            item.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }

                        builder.Attachments.Add(item.FileName, fileBytes, ContentType.Parse(item.ContentType));
                    }
                });
            }

            builder.HtmlBody = mailRequest.body;
            email.Body = builder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(emailSettings.FromMail, emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }

            #region [TODO : SEND RESULT CODE (SAVE RESULT DATA)]

            #endregion
            
            return await Result.SuccessAsync($"send message done : {string.Join(",", email.To.Select(m => m.Name))}");
        }

        public override object ConvertRequest(INotifyMessageRequest request)
        {
            var commonRequest = request as CommonNotifyMessageReqeust;
            if (commonRequest.xIsEmpty()) return null;
            return new EmailNotifyMessageRequest(null, null, null, null);
        }
    }
}