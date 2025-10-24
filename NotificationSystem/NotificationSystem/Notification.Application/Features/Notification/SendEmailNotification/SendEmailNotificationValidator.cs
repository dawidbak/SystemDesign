using FluentValidation;

namespace Notification.Application.Features.Notification.SendEmailNotification;

public class SendEmailNotificationValidator : AbstractValidator<SendEmailNotification>
{
    public SendEmailNotificationValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.TemplateId).NotEmpty();
    }
}