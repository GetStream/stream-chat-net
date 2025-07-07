using System;

namespace StreamChat.Models
{
    public class SharedLocationRequest
    {
        public string CreatedByDeviceId { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string MessageId { get; set; }
    }

    public class SharedLocationResponse : ApiResponse
    {
        public string ChannelCid { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string CreatedByDeviceId { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string MessageId { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string UserId { get; set; }
    }

    public class ActiveLiveLocationsResponse : ApiResponse
    {
        public SharedLocationResponse[] ActiveLiveLocations { get; set; }
    }
}