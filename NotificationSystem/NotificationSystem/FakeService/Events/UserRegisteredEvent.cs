namespace FakeService.Events;

public record UserRegisteredEvent(Guid Id, string Email, int PhoneNumber, int PhoneCode, DeviceInfo? DeviceInfo);
public record DeviceInfo(string DeviceToken, DeviceType Type);
public enum DeviceType
{
    Ios,
    Android,
}