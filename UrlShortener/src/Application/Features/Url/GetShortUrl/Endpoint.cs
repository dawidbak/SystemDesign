using Application.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.Url.GetShortUrl;

public class Endpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder app)
        => app.MapGet("/api/{shortUrl}", async (
                    string shortUrl,
                    [FromServices] IHttpQueryHandler<GetShortUrl> getShortUrlHandler,
                    CancellationToken cancellationToken) =>
                await getShortUrlHandler.HandleAsync(new GetShortUrl(shortUrl), cancellationToken))
            .WithName("GetShortUrl")
            .Produces<string>(StatusCodes.Status301MovedPermanently)
            .Produces(StatusCodes.Status404NotFound);
}