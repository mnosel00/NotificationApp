using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NotificationApp.Domain.Enums;

namespace NotificationApp.Application.Notifications.Commands
{
    public class CreateNotificationCommand : IRequest<Guid>
    {
        public string Content { get; set; } = null!;
        public NotificationChannel Channel { get; set; }
        public string Recipient { get; set; } = null!;
        public DateTime ScheduledTimeLocal { get; set; }
        public string TimeZone { get; set; } = "UTC";
        public NotificationPriority Priority { get; set; } = NotificationPriority.Low;

    }
}
