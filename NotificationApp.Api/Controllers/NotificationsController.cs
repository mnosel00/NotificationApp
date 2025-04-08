using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotificationApp.Application.Notifications.Commands;

namespace NotificationApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
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
            // TODO: implementacja później
            return Ok(new { Id = id });
        }
    }
}
