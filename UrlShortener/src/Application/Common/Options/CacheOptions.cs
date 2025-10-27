namespace Application.Common.Options;

public class CacheOptions
{
    public const string SectionName = "Cache";
    
    public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromHours(24);
}

