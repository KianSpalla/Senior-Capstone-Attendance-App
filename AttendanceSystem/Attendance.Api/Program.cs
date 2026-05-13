using Attendance.Api.Data;
using Microsoft.EntityFrameworkCore;
using Attendance.Api.Queries;
using Attendance.Api.Services;
using System.Text.Json.Serialization;
    
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AttendanceWeb", policy =>
    {
        policy.WithOrigins("http://localhost:5091", "https://localhost:7006")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add Entity Framework services
builder.Services.AddDbContext<AttendanceDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? builder.Configuration.GetConnectionString("AttendanceDb")
        ?? throw new InvalidOperationException("Missing database connection string.");

    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 40)));
});
// Add repositories and services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddSingleton<IEventQrCodeService, EventQrCodeService>();

builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IClassService, ClassService>();

builder.Services.AddScoped<ICheckinRepository, CheckinRepository>();
builder.Services.AddScoped<ICheckInService, CheckInService>();
builder.Services.AddScoped<IAttendanceReportService, AttendanceReportService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AttendanceWeb");

app.UseAuthorization();

app.MapControllers();

app.Run();
