﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationApp.Infrastructure.Data;
using NotificationApp.Scheduler;
using System;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
        });

        services.AddHostedService<SchedulerWorker>();
    })
    .Build();

host.Run();