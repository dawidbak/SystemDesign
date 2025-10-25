namespace Shared.Contracts;

public record EmailNotificationRequestedEvent(string RecipientEmail, string Subject, string Body);