﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NotificationApp.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NotificationChannel
    {
        Email,SMS
    }
}
