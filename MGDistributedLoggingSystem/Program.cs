using Microsoft.EntityFrameworkCore;
using Serilog;
using MGDistributedLoggingSystem.Configurations;
using MGDistributedLoggingSystem.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<S3Config>(builder.Configuration.GetSection("Storage:S3"));
builder.Services.Configure<LocalFileSystemConfig>(builder.Configuration.GetSection("Storage:LocalFileSystem"));
builder.Services.Configure<RabbitMQSenderOptions>(builder.Configuration.GetSection("RabbitMQSenderOptions"));

builder.Services.AddSwaggerGenContactInfo();
builder.Services.AddSwaggerGenAuthorizationButton();
builder.Services.AddApplicationServices(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddApplicationIdentity();
builder.Services.AddApplicationJwtAuth(builder.Configuration.GetSection("Jwt").Get<JwtConfig>());

builder.Services.AddApplicationAuthorization();


//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:51410") // Angular's default port
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});


var app = builder.Build();


await app.AddApplicationDataSeedingAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseCors("AllowAngularApp");
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

Log.Information("Hello, I am Massad Ghanem\r\nYou can communicate with me via\r\nWhatsApp\r\n+201004918459\r\nor LinkedIn\r\nhttps://www.linkedin.com/in/elmagekmosaad/");
