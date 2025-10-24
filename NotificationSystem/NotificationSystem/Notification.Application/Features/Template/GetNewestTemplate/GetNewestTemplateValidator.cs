using FluentValidation;

namespace Notification.Application.Features.Template.GetNewestTemplate;

public class GetNewestTemplateValidator : AbstractValidator<GetNewestTemplate>
{
    public GetNewestTemplateValidator()
    {
        RuleFor(x => x.Type).NotNull();
    }
}