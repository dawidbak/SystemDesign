using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace FakeService.Controllers;

[ApiController]
[Route("[controller]")]
public class FakeController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<FakeController> _logger;

    public FakeController(IPublishEndpoint publishEndpoint, ILogger<FakeController> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser([FromBody] bool addDeviceInfo)
    {
        DeviceInfo? deviceInfo = null;
        if (addDeviceInfo)
        {
            deviceInfo = new DeviceInfo($"device-token-{Guid.NewGuid()}", DeviceType.Android);
        }

        var @event = new UserRegisteredEvent(
            Guid.CreateVersion7(),
            $"{Guid.NewGuid()}@example.com",
            123456789,
            48,
            deviceInfo);
        await _publishEndpoint.Publish(@event);

        return Ok(new
        {
            message = "Event published successfully",
            eventId = @event.Id,
            email = @event.Email
        });
    }

    [HttpPost("create-payment-reminder-created-event")]
    // Note: userId must be real user ID existing in Notification service database
    public async Task<IActionResult> CreatePaymentReminderEvent([FromBody] Guid userId)
    {
        var @event = new PaymentReminderCreatedEvent(
            userId);
        await _publishEndpoint.Publish(@event);

        return Ok(new
        {
            message = "PaymentReminderCreatedEvent published successfully",
        });
    }
}