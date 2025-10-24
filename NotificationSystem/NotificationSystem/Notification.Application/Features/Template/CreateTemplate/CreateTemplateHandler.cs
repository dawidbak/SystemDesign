using Microsoft.AspNetCore.Http;
using Notification.Application.Common;
using Notification.Application.Infrastructure.Repositories;

namespace Notification.Application.Features.Template.CreateTemplate;

public class CreateTemplateHandler : IHttpCommandHandler<CreateTemplate>
{
    private readonly ITemplateRepository _repository;

    public CreateTemplateHandler(ITemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<IResult> HandleAsync(CreateTemplate command, CancellationToken cancellationToken)
    {
        var template = new Domain.Template(
            command.Name,
            command.Content,
            command.Subject,
            command.Type
        );

        var createdTemplate = await _repository.CreateAsync(template, cancellationToken);

        return Results.Ok(createdTemplate.Id);
    }
}