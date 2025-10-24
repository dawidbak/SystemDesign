using FakeService.Events;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace FakeService.Controllers;

[ApiController]
[Route("[controller]")]
public class FakeController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public FakeController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] bool addDeviceInfo)
    {
        var @event =
            new UserRegisteredEvent(Guid.CreateVersion7(), $"{Guid.NewGuid()}@example.com", 123456789, 48, null);
        if (addDeviceInfo)
        {
            @event = @event with
            {
                DeviceInfo = new DeviceInfo($"{Guid.NewGuid()}", DeviceType.Android)
            };
        }
        await _messageBus.PublishAsync(@event);
        return Ok();
    }
}