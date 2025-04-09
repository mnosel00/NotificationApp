﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationApp.Application.Messaging;
using NotificationApp.Domain.Entities;
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
    public class SendNotificationConsumer : IConsumer<SendMessage>
    {
        private readonly NotificationDbContext _context;
        private readonly ILogger<SendNotificationConsumer> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public SendNotificationConsumer(NotificationDbContext context, ILogger<SendNotificationConsumer> logger)
        {
            _context = context;
            _logger = logger;
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2),
                    (ex, time) => _logger.LogWarning($"Retry due to: {ex.Message}"));
        }

        public  async Task Consume(ConsumeContext<SendMessage> context)
        {
            var message = context.Message;

            _logger.LogInformation($"[SENDER] Received notification {message.NotificationId}");

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == message.NotificationId);

            if (notification == null || notification.IsSent)
            {
                _logger.LogWarning($"[SENDER] Notification {message.NotificationId} not found or already sent.");
                return;
            }

            await _retryPolicy.ExecuteAsync(async () =>
            {
                // Tu byłby kod np. do EmailService.Send(...)
                await SimulateSend(notification);
            });

            notification.IsSent = true;
            notification.RetryCount += 1;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"[SENDER] Notification {notification.Id} marked as sent.");
        }

        private Task SimulateSend(Notification n)
        {
            _logger.LogInformation($"[SEND] Sending {n.Channel} to {n.Recipient}: {n.Content}");
            return Task.CompletedTask;
        }
    }
}
