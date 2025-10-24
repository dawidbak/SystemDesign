using Microsoft.AspNetCore.Http;
using Notification.Application.Common;
using Notification.Application.Infrastructure.Repositories;

namespace Notification.Application.Features.Setting.ChangeSetting;

public class ChangeSettingHandler : IHttpCommandHandler<ChangeSetting>
{
    private readonly ISettingRepository _repository;

    public ChangeSettingHandler(ISettingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IResult> HandleAsync(ChangeSetting command, CancellationToken cancellationToken)
    {
        var setting = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (setting == null)
        {
            return Results.NotFound("Setting not found");
        }

        setting.OptIn = command.OptIn;
        await _repository.UpdateAsync(setting, cancellationToken);

        return Results.Ok();
    }
}