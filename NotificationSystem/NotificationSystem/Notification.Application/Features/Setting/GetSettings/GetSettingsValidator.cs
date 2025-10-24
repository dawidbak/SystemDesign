using FluentValidation;

namespace Notification.Application.Features.Setting.GetSettings;

public class GetSettingsValidator : AbstractValidator<GetSettings>
{
    public GetSettingsValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}