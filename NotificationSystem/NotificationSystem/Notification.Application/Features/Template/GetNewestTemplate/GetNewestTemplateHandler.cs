using Microsoft.AspNetCore.Http;
using Notification.Application.Common;
using Notification.Application.Infrastructure.Repositories;

namespace Notification.Application.Features.Template.GetNewestTemplate;

public class GetNewestTemplateHandler : IHttpQueryHandler<GetNewestTemplate>
{
    private readonly ITemplateRepository _repository;

    public GetNewestTemplateHandler(ITemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<IResult> HandleAsync(GetNewestTemplate query, CancellationToken cancellationToken)
    {
        var template = await _repository.GetLatestByTypeAsync(query.Type, cancellationToken);
        
        if (template == null)
        {
            return Results.NotFound("Template not found");
        }

        var templateDto = new GetNewestTemplateDto(
            template.Id, 
            template.Name, 
            template.Subject, 
            template.Content, 
            template.Type, 
            template.CreatedAt
        );
        
        return Results.Ok(templateDto);
    }
}
