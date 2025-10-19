using Microsoft.AspNetCore.Routing;

namespace Application.Common;

public interface IEndpoint
{
     void RegisterEndpoint(IEndpointRouteBuilder app);
}