using NotificationApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationApp.Application.Messaging
{
    public class SendMessage
    {
        public Guid NotificationId { get; set; }
        public NotificationChannel Channel { get; set; }
        public NotificationPriority Priority { get; set; }

    }
}
