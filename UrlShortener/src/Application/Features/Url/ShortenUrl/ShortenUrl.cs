using Application.Common;

namespace Application.Features.Url.ShortenUrl;

public record ShortenUrl(string OriginalUrl) : IHttpCommand;