using System.Numerics;

namespace Application.Domain;

public class UrlMapping
{
    public required ulong Id { get; set; }
    public required string ShortUrl { get; set; }
    public required string OriginalUrl { get; set; }
}