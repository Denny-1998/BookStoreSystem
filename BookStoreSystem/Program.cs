using System.Text.Json.Serialization;
using BookStoreSystem;
using BookStoreSystem.Model;
using BookStoreSystem.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

DbSettings dbSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DbSettings>();
builder.Services.AddSingleton(dbSettings);

builder.Services.AddDbContext<BookStoreDbContext>(options => options.UseSqlServer(dbSettings.SqlConnectionString));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(dbSettings.RedisConnectionString));

builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<AuthorService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
