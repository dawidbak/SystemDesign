using Notification.Application.Domain;

namespace Notification.Application.Features.Setting.GetSettings;

internal record GetSettingDto(Guid Id, ChannelType Channel, bool OptIn);