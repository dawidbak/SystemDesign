namespace NotificationJob.Services;

public interface IFakeEmailService
{
    Task SendEmail(string recipientEmail, string subject, string body);
}

public class FakeEmailService : IFakeEmailService
{
    public async Task SendEmail(string recipientEmail, string subject, string body)
    {
        await Task.Delay(10);
        Console.WriteLine($"Email sent to {recipientEmail} with subject '{subject}' and body '{body}'");
    }
}