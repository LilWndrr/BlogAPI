using BlogAPI.DTOs;

namespace BlogAPI.Services.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(Message message);
}