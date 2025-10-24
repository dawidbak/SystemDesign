using FluentValidation;

namespace Notification.Application.Features.Template.CreateTemplate;

public class CreateTemplateValidator : AbstractValidator<CreateTemplate>
{
    public CreateTemplateValidator()
    {
        RuleFor(x => x.Type).NotNull();
    }
}