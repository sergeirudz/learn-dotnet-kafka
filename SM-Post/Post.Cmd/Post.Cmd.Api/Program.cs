using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.infrastructure;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Order is important
builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>(); // Depends on MongoDB
builder.Services.AddScoped<IEventStore, EventStore>(); // Depends on IEventStoreRepository
builder.Services.AddScoped<IEventSourcingHandler<PostAggregate>,EventSourcingHandler>(); // Depends on IEventStore
builder.Services.AddScoped<ICommandHandler, CommandHandler>(); // Depends on IEventSourcingHandler

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
