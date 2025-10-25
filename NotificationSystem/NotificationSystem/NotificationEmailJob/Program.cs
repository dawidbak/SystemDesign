using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationJob.Handlers;
using NotificationJob.Infrastructure;
using NotificationJob.Infrastructure.Repositories;
using NotificationJob.Outbox;
using NotificationJob.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<NotificationEmailJobDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmailNotificationRequestedEventHandler>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration.GetConnectionString("RabbitMQ")!));
        
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddHostedService<OutboxProcessorJob>();
builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
builder.Services.AddScoped<IInboxMessageRepository, InboxMessageRepository>();
builder.Services.AddScoped<IFakeEmailService, FakeEmailService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();