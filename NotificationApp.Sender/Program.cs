using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationApp.Infrastructure.Data;
using NotificationApp.Sender;
using NotificationApp.Sender.Consumers;
using System;

 Host.CreateDefaultBuilder(args)
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

                cfg.ReceiveEndpoint("notification-send", e =>
                {
                    e.ConfigureConsumer<SendNotificationConsumer>(ctx);
                });
            });
        });

        services.AddMassTransitHostedService();
    })
    .Build()
    .Run();