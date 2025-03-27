using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class ModerationPayload
    {
        [JsonProperty("texts")]
        public List<string> Texts { get; set; }

        [JsonProperty("images")]
        public List<string> Images { get; set; }

        [JsonProperty("videos")]
        public List<string> Videos { get; set; }

        [JsonProperty("custom")]
        public Dictionary<string, object> Custom { get; set; }
    }

    public class ModerationCheckOptions
    {
        [JsonProperty("force_sync")]
        public bool? ForceSync { get; set; }

        [JsonProperty("test_mode")]
        public bool? TestMode { get; set; }
    }

    public class UserProfileCheckRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }
    }

    public class ModerationCheckResponse
    {
        [JsonProperty("recommended_action")]
        public string RecommendedAction { get; set; }

        // Add other response fields as needed based on the API response
    }

    public static class ModerationEntityTypes
    {
        public const string User = "stream:user";
        public const string Message = "stream:chat:v1:message";
        public const string UserProfile = "stream:v1:user_profile";
    }
} 