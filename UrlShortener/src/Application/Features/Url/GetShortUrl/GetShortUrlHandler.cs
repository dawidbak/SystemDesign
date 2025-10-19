using Application.Common;
using Application.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Url.GetShortUrl;

public class GetShortUrlHandler : IHttpQueryHandler<GetShortUrl>
{
    private readonly IUrlMappingRepository _urlMappingRepository;

    public GetShortUrlHandler(IUrlMappingRepository urlMappingRepository)
    {
        _urlMappingRepository = urlMappingRepository;
    }

    public async Task<IResult> HandleAsync(GetShortUrl query, CancellationToken cancellationToken)
    {
        if (!IsValidShortUrl(query.ShortUrl))
        {
            return Results.BadRequest("Invalid short URL format");
        }

        var urlMapping = await _urlMappingRepository.GetByShortUrl(query.ShortUrl, cancellationToken);
        return urlMapping != null ? Results.Redirect(urlMapping.OriginalUrl) : Results.NotFound("Short URL not found");
    }

    private static bool IsValidShortUrl(string shortUrl)
    {
        return shortUrl switch
        {
            null => false,
            _ when string.IsNullOrWhiteSpace(shortUrl) => false,
            // Length 11 because of Base64 encoding of Snowflake ID
            _ when shortUrl.Length != 11 => false,
            _ when !shortUrl.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_') => false,
            _ => true
        };
    }
}