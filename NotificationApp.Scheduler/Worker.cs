using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationApp.Application.Messaging;
using NotificationApp.Infrastructure.Data;
using System;

namespace NotificationApp.Scheduler
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _publishEndpoint = publishEndpoint;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

                var dueNotifications = await db.Notifications
                    .Where(n => !n.IsSent && n.ScheduledTimeUtc <= DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                foreach (var notification in dueNotifications)
                {
                    var message = new SendMessage
                    {
                        NotificationId = notification.Id,
                        Channel = notification.Channel.ToString()
                    };

                    _logger.LogInformation($"[SCHEDULER] Publishing notification {notification.Id}");

                    await _publishEndpoint.Publish(message, stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // sprawdzaj co 10 sekund
            }
        }
    }
}
