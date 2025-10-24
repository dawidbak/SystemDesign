using JasperFx.Core;
using Notification.Application;
using Notification.Application.Common;
using Notification.Application.Features.Notification.Events;
using Notification.Application.Features.User.Events;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.MapApplication(builder.Configuration);
builder.UseWolverine(opts =>
{
    opts.UseRabbitMq(new Uri(builder.Configuration.GetSection("ConnectionStrings")["RabbitMQ"]))
        .DeclareExchange("notificationExchange",
            exchange => { exchange.BindQueue("NotificationEmailQueue", nameof(EmailNotificationRequestedEvent)); });
        // .AutoProvision();

    opts.ListenToRabbitQueue("UserQueue")
        .PreFetchCount(100)
        .ListenerCount(5)
        .CircuitBreaker(cb =>
        {
            cb.PauseTime = 1.Minutes();
            cb.FailurePercentageThreshold = 10;
        })
        .UseDurableInbox();

    opts.UseFluentValidation();

    opts.PublishAllMessages().ToRabbitExchange("notificationExchange");
    opts.ApplicationAssembly = typeof(ConfigureServices).Assembly;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.RegisterEndpoints();

app.UseHttpsRedirection();
app.Run();

