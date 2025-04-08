using NotificationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationApp.Domain.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification, CancellationToken cancellationToken);
    }
}
