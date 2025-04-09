using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationApp.Application.Messaging;
using NotificationApp.Infrastructure.Data;
using System;

namespace NotificationApp.Scheduler
{
    public class SchedulerWorker : BackgroundService
    {
        private readonly ILogger<SchedulerWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        

        public SchedulerWorker(ILogger<SchedulerWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

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

                    await publishEndpoint.Publish(message, stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
