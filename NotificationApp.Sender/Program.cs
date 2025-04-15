using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationApp.Infrastructure.Data;
using NotificationApp.Sender;
using NotificationApp.Sender.Consumers;
using System;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

        services.AddMassTransit(x =>
        {
            x.AddConsumer<SendNotificationConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("notification-send-email", e =>
                {
                    e.ConfigureConsumer<SendNotificationConsumer>(ctx);
                });

                cfg.ReceiveEndpoint("notification-send-sms", e =>
                {
                    e.ConfigureConsumer<SendNotificationConsumer>(ctx);
                });
            });
        });

        
    }) 
    .Build()
    .RunAsync();