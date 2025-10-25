using MassTransit;
using Notification.Application.Domain;
using Notification.Application.Infrastructure.Repositories;
using Shared.Contracts;

namespace Notification.Application.Features.User.IngestUser;

public class UserRegisteredEventHandler : IConsumer<UserRegisteredEvent>
{
    private readonly IUserRepository _repository;

    public UserRegisteredEventHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var message = context.Message;
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

        var domainDeviceType = deviceInfo.Type switch
        {
            Shared.Contracts.DeviceType.Ios => Domain.DeviceType.Ios,
            Shared.Contracts.DeviceType.Android => Domain.DeviceType.Android,
            _ => throw new ArgumentOutOfRangeException(nameof(deviceInfo.Type))
        };

        var device = new Device(
            Guid.CreateVersion7(),
            deviceInfo.DeviceToken,
            user.Id,
            TimeProvider.System.GetUtcNow().UtcDateTime,
            domainDeviceType);
        user.Devices = new List<Device> { device };
    }

    private static void ApplyDefaultSettings(Domain.User user)
    {
        var emailSetting = new Domain.Setting(user.Id, ChannelType.Email, true,
            Guid.CreateVersion7());
        var smsSetting = new Domain.Setting(user.Id, ChannelType.Sms, true,
            Guid.CreateVersion7());
        var pushSetting = new Domain.Setting(user.Id, ChannelType.PushNotification, true,
            Guid.CreateVersion7());
        
        user.Settings = new List<Domain.Setting>
        {
            emailSetting,
            smsSetting,
            pushSetting
        };
    }
}