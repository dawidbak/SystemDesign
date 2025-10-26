namespace Notification.Application.Domain;

public class Template
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public ChannelType Type { get; set; }
    public DateTime CreatedAt { get; set; }

    internal Template(string name, string content, string subject ,ChannelType type)
    {
        Name = name;
        Content = content;
        Type = type;
        Subject = subject;
        CreatedAt = TimeProvider.System.GetUtcNow().UtcDateTime;
    }
    
    public Template(){}
}