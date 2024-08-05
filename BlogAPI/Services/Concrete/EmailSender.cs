
using BlogAPI.DTOs;
using BlogAPI.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BlogAPI.Services.Concrete;

public class EmailSender:IEmailSender
{
    
    private readonly EmailConfiguration _emailConfig;
    public EmailSender(EmailConfiguration emailConfig)
    {
        _emailConfig = emailConfig;
    }


    public async Task SendEmailAsync(Message message)
    {
        var mailMessage = CreateEmailMessage(message);

        await SendAsync(mailMessage);
    }
    private async Task SendAsync(MimeMessage mailMessage)
    {
        
        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                await client.SendAsync(mailMessage);
            }
            catch
            {
                //log an error message or throw an exception, or both.
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
        
    }
    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Address",_emailConfig.From));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };

        return emailMessage;
    }
}