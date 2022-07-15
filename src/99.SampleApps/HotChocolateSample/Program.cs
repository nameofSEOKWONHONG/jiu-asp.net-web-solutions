using HotChocolateSample.Entity;
using HotChocolateSample.GraphQL;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<BookService>();
builder.Services.AddInMemorySubscriptions();
//if use redis subscription
//dotnet add package HotChocolate.Subscriptions.Redis
//builder.Services.AddRedisSubscriptions((sp) => ConnectionMultiplexer.Connect("host:port"));
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapGraphQL();
// });

app.MapGraphQL();

app.Run();