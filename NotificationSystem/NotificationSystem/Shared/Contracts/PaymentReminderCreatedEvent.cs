namespace Shared.Contracts;


// Contains only userId because  it's a example of event between services
public record PaymentReminderCreatedEvent(Guid UserId);