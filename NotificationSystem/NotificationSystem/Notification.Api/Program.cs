using MassTransit;
using Notification.Application;
using Notification.Application.Common;
using Notification.Application.Features.User.IngestUser;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.MapApplication(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserRegisteredEventHandler>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration.GetConnectionString("RabbitMQ")!));
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.RegisterEndpoints();
app.UseHttpsRedirection();

app.Run();
