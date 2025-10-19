using Application.Common;

namespace Application.Features.Url.GetShortUrl;

public record GetShortUrl(string ShortUrl) : IHttpQuery;