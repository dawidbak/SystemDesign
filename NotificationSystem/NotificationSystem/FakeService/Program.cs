using FakeService.Events;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.UseWolverine(opts =>
{
    opts.UseRabbitMq(new Uri(builder.Configuration.GetSection("ConnectionStrings")["RabbitMQ"]))
        .DeclareExchange("UserExchange",
            exchange => { exchange.BindQueue("UserQueue", nameof(UserRegisteredEvent)); }).AutoProvision();

    opts.PublishMessage<UserRegisteredEvent>().ToRabbitQueue("UserQueue");
    // opts.PublishMessage<UserRegisteredEvent>().ToRabbitExchange("UserExchange");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();