using System;
using System.Collections.Generic;

namespace StreamChat.Models
{
    public class Device
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public string Id { get; set; }
        public string PushProvider { get; set; }
        public string PushProviderName { get; set; }
        public string UserId { get; set; }
        public bool? Disabled { get; set; }
        public string DisabledReason { get; set; }
    }

    public class GetDevicesResponse : ApiResponse
    {
        public List<Device> Devices { get; set; }
    }
}
