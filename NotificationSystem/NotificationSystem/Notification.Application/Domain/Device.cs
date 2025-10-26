namespace Notification.Application.Domain;

public class Device
{
    public Guid Id { get; set; }
    public string DeviceToken { get; set; }
    public Guid UserId { get; set; }
    public DateTime LastLoggedInAt { get; set; }
    public DeviceType Type { get; set; }
    
    internal Device(Guid id, string deviceToken, Guid userId, DateTime lastLoggedInAt, DeviceType type)
    {
        Id = id;
        DeviceToken = deviceToken;
        UserId = userId;
        LastLoggedInAt = lastLoggedInAt;
        Type = type;
    }
    
    public Device(){}
}