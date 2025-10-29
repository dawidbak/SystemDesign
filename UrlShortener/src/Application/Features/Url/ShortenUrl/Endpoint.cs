using Application.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.Url.ShortenUrl;

public class Endpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder app)
        => app.MapPost("/", async (
                    ShortenUrl shortenUrl,
                    [FromServices] IHttpCommandHandler<ShortenUrl> shortenUrlHandler,
                    CancellationToken cancellationToken) =>
                await shortenUrlHandler.HandleAsync(shortenUrl, cancellationToken))
            .WithName("ShortenUrl")
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
}