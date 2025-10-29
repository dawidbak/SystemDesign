using Application.Common;
using Application.Common.Options;
using Application.Domain;
using Application.Infrastructure.Repositories;
using Application.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Features.Url.ShortenUrl;

public class ShortenUrlHandler : IHttpCommandHandler<ShortenUrl>
{
    private readonly ISnowflakeService _snowflakeService;
    private readonly ShortenerOptions _options;
    private readonly IUrlMappingRepository _urlMappingRepository;


    public ShortenUrlHandler(ISnowflakeService snowflakeService, IOptions<ShortenerOptions> options,
        IUrlMappingRepository urlMappingRepository)
    {
        _snowflakeService = snowflakeService;
        _urlMappingRepository = urlMappingRepository;
        _options = options.Value;
    }

    public async Task<IResult> HandleAsync(ShortenUrl query, CancellationToken cancellationToken)
    {
        if (!IsValidOriginalUrl(query.OriginalUrl))
        {
            return Results.BadRequest("Invalid original URL format");
        }
        
        var urlMapping = await _urlMappingRepository.GetByOriginalUrl(query.OriginalUrl, cancellationToken);
        if (urlMapping != null)
        {
            return Results.Ok(new ShortenUrlDto($"{_options.BaseUrl}{urlMapping.ShortUrl}", urlMapping.OriginalUrl));
        }

        var id = _snowflakeService.GenerateId();
        var shortUrl = GenerateShortUrl(id);

        urlMapping = new UrlMapping
        {
            Id = id,
            OriginalUrl = query.OriginalUrl,
            ShortUrl = shortUrl
        };

        await _urlMappingRepository.Create(urlMapping, cancellationToken);
        var fullShortUrl = new System.Text.StringBuilder(_options.BaseUrl);
        fullShortUrl.Append(shortUrl);

        var shortenUrlDto = new ShortenUrlDto
        (
            fullShortUrl.ToString(),
            query.OriginalUrl
        );
        return Results.Created(fullShortUrl.ToString(), shortenUrlDto);
    }
    
    private static string GenerateShortUrl(ulong id)
    {
        var bytes = BitConverter.GetBytes(id);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
    
    private static bool IsValidOriginalUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}