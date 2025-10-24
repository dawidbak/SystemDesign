using Notification.Application.Common;
using Notification.Application.Domain;

namespace Notification.Application.Features.Template.GetNewestTemplate;

public record GetNewestTemplate(ChannelType Type) : IHttpQuery;

