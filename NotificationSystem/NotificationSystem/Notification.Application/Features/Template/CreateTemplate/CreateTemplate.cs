using Notification.Application.Common;
using Notification.Application.Domain;

namespace Notification.Application.Features.Template.CreateTemplate;

public record CreateTemplate(string Name, string Subject, string Content, ChannelType Type) : IHttpCommand;