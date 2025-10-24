using Microsoft.AspNetCore.Http;
using Notification.Application.Common;
using Notification.Application.Infrastructure.Repositories;

namespace Notification.Application.Features.Setting.GetSettings;

public class GetSettingsHandler : IHttpQueryHandler<GetSettings>
{
    private readonly ISettingRepository _repository;

    public GetSettingsHandler(ISettingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IResult> HandleAsync(GetSettings query, CancellationToken cancellationToken)
    {
        var settings = await _repository.GetAllByUserIdAsync(query.UserId, cancellationToken);
        var settingDtos = MapToGetSettingDtos(settings);
        return Results.Ok(settingDtos);
    }

    private List<GetSettingDto> MapToGetSettingDtos(List<Domain.Setting> settings)
    {
        return settings.Select(s => new GetSettingDto(s.Id, s.Channel, s.OptIn))
            .ToList();
    }
}