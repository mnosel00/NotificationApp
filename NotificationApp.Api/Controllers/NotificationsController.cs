using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationApp.Application.Notifications.Commands;
using NotificationApp.Domain.Entities;
using NotificationApp.Infrastructure.Data;

namespace NotificationApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly NotificationDbContext _context;

        public NotificationsController(IMediator mediator, NotificationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNotificationCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(new { Id = id });
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var now = DateTime.UtcNow;

            var notifications = await _context.Notifications
                .Select(n => new
                {
                    n.Id,
                    n.Recipient,
                    n.Content,
                    n.Channel,
                    n.ScheduledTimeUtc,
                    Status = GetNotificationStatus(n),
                })
                .ToListAsync();

            return Ok(notifications);
        }

        private static string GetNotificationStatus(Notification n)
        {
            if (n.IsSent)
            {
                return "Wysłane";
            }
            else if (!n.IsSent && n.RetryCount >= 3)
            {
                return "Nieudane";
            }
            else
            {
                return "Oczekujące";
            }
        }
    }
}
