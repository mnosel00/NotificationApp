using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace NotificationApp.Application.Notifications.Commands
{
    public class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
    {
        public CreateNotificationCommandValidator()
        {
            RuleFor(x => x.Content).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.Recipient).NotEmpty().EmailAddress();
            RuleFor(x => x.TimeZone).NotEmpty();
            RuleFor(x => x.ScheduledTimeLocal).GreaterThan(DateTime.Now);
        }
    }
}
