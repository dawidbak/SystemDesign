using FluentValidation;
using Notification.Application.Features.User.Events;

namespace Notification.Application.Features.User.IngestUser;

public class UserRegisteredEventValidator : AbstractValidator<UserRegisteredEvent>
{
    public UserRegisteredEventValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.PhoneCode).NotEmpty();
    }
    
}