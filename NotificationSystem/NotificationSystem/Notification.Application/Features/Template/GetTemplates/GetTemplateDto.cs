using Notification.Application.Domain;

namespace Notification.Application.Features.Template.GetTemplates;

internal record GetTemplateDto(int Id, string Name, string Subject, string Content, ChannelType Type, DateTime CreatedAt);

