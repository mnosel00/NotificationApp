using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationApp.Application.Messaging;
using NotificationApp.Domain.Entities;
using NotificationApp.Domain.Enums;
using NotificationApp.Infrastructure.Data;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationApp.Sender.Consumers
{
    public class SendEmailConsumer : IConsumer<SendMessage>
    {
        private readonly NotificationDbContext _context;
        private readonly ILogger<SendEmailConsumer> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public SendEmailConsumer(NotificationDbContext context, ILogger<SendEmailConsumer> logger)
        {
            _context = context;
            _logger = logger;
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2),
                   async (exception, timeSpan, retryCount, context) =>
                   {
                       _logger.LogWarning($"[EMAIL] Retry {retryCount} due to: {exception.Message}");

                       if (context.ContainsKey("notification"))
                       {
                           var notification = (Notification)context["notification"];
                           notification.RetryCount += 1;
                           await _context.SaveChangesAsync();
                       }
                   });
        }

        public async Task Consume(ConsumeContext<SendMessage> context)
        {
            if (context.Message.Channel != NotificationChannel.Email)
                return;

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == context.Message.NotificationId);

            if (notification == null || notification.IsSent)
            {
                _logger.LogWarning($"EMAIL Notification {context.Message.NotificationId} not found or already sent.");
                return;
            }

            await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation($"EMAIL Sending email to {notification.Recipient}: {notification.Content}");
                await SimulateSend(notification);
            });

            notification.IsSent = true;
            notification.ForceSend = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"EMAIL Notification {notification.Id} marked as sent.");
        }

        private Task SimulateSend(Notification n)
        {
            var success = new Random().NextDouble() < 0.5;

            if (!success)
                throw new Exception("failure");

            _logger.LogInformation($"EMAIL Simulated success: {n.Channel} to {n.Recipient}: {n.Content}");
            return Task.CompletedTask;
        }
    }
    
}

