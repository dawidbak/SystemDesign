using Microsoft.Extensions.Options;

namespace Application.Common.Options;

public class ShortenerOptions
{
    public const string Shortener = "Shortener";
    public string BaseUrl { get; set; }
}