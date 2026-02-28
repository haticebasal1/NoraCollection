using System;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Configurations.Email;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class EmailManager : IEmailService
{
    private readonly EmailConfig _emailConfig;

    public EmailManager(IOptions<EmailConfig> emailConfig)
    {
        _emailConfig = emailConfig.Value;
    }

    public async Task<ResponseDto<bool>> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body))
            {
                return ResponseDto<bool>.Fail("Alıcı, konu veya içerik boş olamaz!",StatusCodes.Status400BadRequest);
            }
            using var smptClient = new SmtpClient(_emailConfig.SmtpHost,_emailConfig.SmtpPort)
            {
                EnableSsl = _emailConfig.EnableSsl,
                Credentials = new NetworkCredential(_emailConfig.UserName,_emailConfig.Password)
            };

            using var mailMessage = new MailMessage
            {
              From=new MailAddress(_emailConfig.SenderEmail,_emailConfig.SenderName),
              Subject = subject,
              Body = body,
              IsBodyHtml = isHtml  
            };
            mailMessage.To.Add(to);
            await smptClient.SendMailAsync(mailMessage);
            return ResponseDto<bool>.Success(true,StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail($"E-posta gönderilemedi!:{ex.Message}",StatusCodes.Status500InternalServerError);
        }
    }
}
