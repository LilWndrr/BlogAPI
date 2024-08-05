using MimeKit;
using MailKit.Net.Smtp;

namespace BlogAPI.DTOs;

public class Message
{
    public List<MailboxAddress> To { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public Message(IEnumerable<string> to, string subject, string content)
    {
        To = new List<MailboxAddress>();
        To.AddRange(to.Select(address => new MailboxAddress("Address",address)));
        Subject = subject;
        Content = content;        
    }
}