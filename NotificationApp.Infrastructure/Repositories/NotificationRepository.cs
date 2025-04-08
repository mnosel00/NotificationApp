using NotificationApp.Domain.Entities;
using NotificationApp.Domain.Repositories;
using NotificationApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationApp.Infrastructure.Repositories
{

    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Notification notification, CancellationToken cancellationToken)
        {
            await _context.Notifications.AddAsync(notification, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
