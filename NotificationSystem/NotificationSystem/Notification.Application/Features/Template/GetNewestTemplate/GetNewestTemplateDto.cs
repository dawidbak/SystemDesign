using Notification.Application.Domain;

namespace Notification.Application.Features.Template.GetNewestTemplate;

internal record GetNewestTemplateDto(int Id, string Name, string? Subject, string Content, ChannelType Type, DateTime CreatedAt);

