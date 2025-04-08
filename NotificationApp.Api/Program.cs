using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotificationApp.Application.Notifications.Commands;
using NotificationApp.Domain.Repositories;
using NotificationApp.Infrastructure.Data;
using NotificationApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateNotificationCommandHandler).Assembly));


builder.Services.AddValidatorsFromAssemblyContaining<CreateNotificationCommandValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();


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

app.Run();
