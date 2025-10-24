using FluentValidation;

namespace Notification.Application.Features.Setting.ChangeSetting;

public class ChangeSettingHandlerValidator : AbstractValidator<ChangeSetting>
{
    public ChangeSettingHandlerValidator()
    {
        RuleFor(x => x.OptIn).NotNull();
        RuleFor(x => x.Id).NotEmpty();
    }
}