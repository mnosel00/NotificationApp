using NotificationApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationApp.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = null!;
        public NotificationChannel Channel { get; set; }
        public string Recipient { get; set; } = null!;
        public DateTime ScheduledTimeUtc { get; set; }
        public string TimeZone { get; set; } = "UTC";

        public bool IsSent { get; set; } = false;
        public int RetryCount { get; set; } = 0;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
