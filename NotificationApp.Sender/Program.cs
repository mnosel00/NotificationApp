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
            x.AddConsumer<SendEmailConsumer>();
            x.AddConsumer<SendSMSConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("notification-send-EMAIL", e =>
                {
                    e.SetQueueArgument("x-max-priority", 10);
                    e.ConfigureConsumer<SendEmailConsumer>(ctx);
                });

                cfg.ReceiveEndpoint("notification-send-SMS", e =>
                {
                    e.SetQueueArgument("x-max-priority", 10);
                    e.ConfigureConsumer<SendSMSConsumer>(ctx);
                });
            });
        });


    }) 
    .Build()
    .RunAsync();


// Wyświetla wiadomość podwójnie
// Mark As sent wyświetla się podwójnie
//