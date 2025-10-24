using Notification.Application.Domain;

namespace Notification.Application.Features.User.Events;

public record UserRegisteredEvent(Guid Id, string Email, int PhoneNumber, int PhoneCode, DeviceInfo? DeviceInfo);

public record DeviceInfo(string DeviceToken, DeviceType Type);