namespace NotificationJob.Inbox;

public class InboxMessage
{
    public Guid Id { get; init; }
    public DateTime OccurredOnUtc { get; init; }
    public DateTime? ProcessedOnUtc { get; init; }
}