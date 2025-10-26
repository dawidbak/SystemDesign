namespace Notification.Application.Domain;

public class Setting
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ChannelType Channel { get; set; }
    public bool OptIn { get; set; }

    internal Setting(Guid userId, ChannelType channel, bool optIn, Guid id)
    {
        Id = id;
        UserId = userId;
        Channel = channel;
        OptIn = optIn;
    }
    
    public Setting(){}

    public void ChangeOptIn()
    {
        OptIn = !OptIn;
    }
}