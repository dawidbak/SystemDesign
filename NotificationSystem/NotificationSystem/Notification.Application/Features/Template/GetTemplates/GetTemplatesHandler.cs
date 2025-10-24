using Microsoft.AspNetCore.Http;
using Notification.Application.Common;
using Notification.Application.Infrastructure.Repositories;

namespace Notification.Application.Features.Template.GetTemplates;

public class GetTemplatesHandler : IHttpQueryHandler<GetTemplatesQuery>
{
    private readonly ITemplateRepository _repository;

    public GetTemplatesHandler(ITemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<IResult> HandleAsync(GetTemplatesQuery query, CancellationToken cancellationToken)
    {
        var templates = await _repository.GetAllAsync(cancellationToken);
        return Results.Ok(MapToGetTemplateDtos(templates));
    }

    private static List<GetTemplateDto> MapToGetTemplateDtos(List<Domain.Template> templates)
    {
        return templates.Select(t => new GetTemplateDto(t.Id, t.Name, t.Subject, t.Content, t.Type, t.CreatedAt))
            .ToList();
    }
}