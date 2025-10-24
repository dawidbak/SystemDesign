using Notification.Application.Common;

namespace Notification.Application.Features.Notification.SendEmailNotification;

public record SendEmailNotification(Guid UserId, int TemplateId) : IHttpCommand;