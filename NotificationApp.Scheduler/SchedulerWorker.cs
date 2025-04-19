using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationApp.Application.Messaging;
using NotificationApp.Domain.Entities;
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
        private bool IsInNightHours(DateTime localTime)
        {
            var hour = localTime.Hour;
            return hour < 6 || hour >= 22;
        }
        private bool IsInWeekend(DateTime localTime)
        {
            var dayOfWeek = localTime.DayOfWeek;
            return dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var dueNotifications = await db.Notifications
                    .Where(n => !n.IsSent && !n.IsCanceled &&
                        (n.ScheduledTimeUtc <= DateTime.UtcNow || n.ForceSend))
                    .ToListAsync(stoppingToken);

                foreach (var notification in dueNotifications)
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(notification.TimeZone);
                    var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                    if (!notification.ForceSend)
                    {
                        if (IsInNightHours(localTime))
                        {
                            _logger.LogInformation($"[SCHEDULER] Skipping {notification.Id} - nocne godziny ({localTime:HH:mm})");
                            continue;
                        }

                        if (IsInWeekend(localTime))
                        {
                            _logger.LogInformation($"[SCHEDULER] Skipping {notification.Id} - weekend ({localTime:dddd})");
                            continue;
                        }
                    }

                    var message = new SendMessage
                    {
                        NotificationId = notification.Id,
                        Channel = notification.Channel.ToString(),
                        Priority = notification.Priority
                    };

                    _logger.LogInformation($"[SCHEDULER] Publishing notification {notification.Id}");

                    await publishEndpoint.Publish(message, stoppingToken);
                   
                    if (notification.ForceSend)
                    {
                        notification.ForceSend = false;
                        await db.SaveChangesAsync();
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
