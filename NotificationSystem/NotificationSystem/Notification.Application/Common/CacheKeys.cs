using Notification.Application.Domain;

namespace Notification.Application.Common;

public static class CacheKeys
{
    public const string UserPrefix = "user_";
    public const string TemplatePrefix = "template_";
    
    // User cache keys
    public static string User(Guid userId) => $"{UserPrefix}{userId}";
    
    // Template cache keys
    public static string Template(int templateId) => $"{TemplatePrefix}{templateId}";
    public static string TemplateAll() => $"{TemplatePrefix}all";
    public static string TemplateLatestByType(ChannelType type) => $"{TemplatePrefix}latest_{type}";
}

