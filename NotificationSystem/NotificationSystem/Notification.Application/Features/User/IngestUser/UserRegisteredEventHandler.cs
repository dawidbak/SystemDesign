using Notification.Application.Features.User.Events;
using Rebus.Handlers;
using Notification.Application.Domain;
using Notification.Application.Infrastructure.Repositories;

namespace Notification.Application.Features.User.IngestUser;

public class UserRegisteredEventHandler : IHandleMessages<UserRegisteredEvent>
{
    private readonly IUserRepository _repository;

    public UserRegisteredEventHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UserRegisteredEvent message)
    {
        var user = new Domain.User(
            message.Id,
            message.Email,
            message.PhoneCode,
            message.PhoneNumber
        );

        AddToDevice(message.DeviceInfo, user);
        ApplyDefaultSettings(user);
        await _repository.AddAsync(user, CancellationToken.None);
    }

    private static void AddToDevice(DeviceInfo? deviceInfo, Domain.User user)
    {
        if (deviceInfo is null)
        {
            return;
        }

        var device = new Device(Guid.CreateVersion7(), deviceInfo.DeviceToken, user.Id,
            TimeProvider.System.GetUtcNow().UtcDateTime, deviceInfo.Type);
        user.Devices.Add(device);
    }

    private static void ApplyDefaultSettings(Domain.User user)
    {
        var emailSetting = new Domain.Setting(user.Id, ChannelType.Email, true,
            Guid.CreateVersion7());
        var smsSetting = new Domain.Setting(user.Id, ChannelType.Sms, true,
            Guid.CreateVersion7());
        var pushSetting = new Domain.Setting(user.Id, ChannelType.PushNotification, true,
            Guid.CreateVersion7());

        user.Settings.Add(emailSetting);
        user.Settings.Add(smsSetting);
    }
}