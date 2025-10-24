namespace Notification.Application.Features.Notification.Events;

public record EmailNotificationRequestedEvent(string RecipientEmail, string Subject, string Body);