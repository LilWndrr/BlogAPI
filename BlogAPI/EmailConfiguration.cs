using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlogAPI;

public class EmailConfiguration
{
    public EmailConfiguration(string from, string smtpServer, int port, string userName, string password)
    {
        From = from;
        SmtpServer = smtpServer;
        Port = port;
        UserName = userName;
        Password = password;
    }

    public EmailConfiguration()
    {
        
    }

    public string From { get; set; }
    public string SmtpServer { get; set; }
    public int Port { get; set; }

    public bool EnableSSl { get; set; }
    
    public string UserName { get; set; }
    public string Password { get; set; }
}