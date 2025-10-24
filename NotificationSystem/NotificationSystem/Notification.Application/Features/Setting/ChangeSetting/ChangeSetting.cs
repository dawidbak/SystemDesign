using Notification.Application.Common;

namespace Notification.Application.Features.Setting.ChangeSetting;

public record ChangeSetting(Guid Id, bool OptIn) : IHttpCommand;