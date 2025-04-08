using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NotificationApp.Domain.Entities;
using NotificationApp.Domain.Repositories;
using TimeZoneConverter;

namespace NotificationApp.Application.Notifications.Commands
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Guid>
    {
        private readonly INotificationRepository _repository;

        public CreateNotificationCommandHandler(INotificationRepository repository)
        {
            _repository = repository;
        }
        public async Task<Guid> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var tz = TZConvert.GetTimeZoneInfo(request.TimeZone);
            var scheduledUtc = TimeZoneInfo.ConvertTimeToUtc(request.ScheduledTimeLocal, tz);

            var notification = new Notification
            {
                Content = request.Content,
                Channel = request.Channel,
                Recipient = request.Recipient,
                ScheduledTimeUtc = scheduledUtc,
                TimeZone = request.TimeZone
            };

            await _repository.AddAsync(notification, cancellationToken);

            return notification.Id;
        }
    }
}
