using Notification.Application.Common;

namespace Notification.Application.Features.Setting.GetSettings;

public record GetSettings(Guid UserId) : IHttpQuery;