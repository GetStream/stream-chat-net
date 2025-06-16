using System.Collections.Generic;

namespace StreamChat.Models
{
    public class ModerationPayload
    {
        public List<string> Texts { get; set; }
        public List<string> Images { get; set; }
        public List<string> Videos { get; set; }
        public Dictionary<string, object> Custom { get; set; }
    }

    public class ModerationCheckOptions
    {
        public bool? ForceSync { get; set; }
        public bool? TestMode { get; set; }
    }

    public class UserProfileCheckRequest
    {
        public string Username { get; set; }
        public string Image { get; set; }
    }

    public class ModerationCheckResponse : ApiResponse
    {
        public string RecommendedAction { get; set; }
        public string Status { get; set; }
    }

    public static class ModerationEntityTypes
    {
        public const string User = "stream:user";
        public const string Message = "stream:chat:v1:message";
        public const string UserProfile = "stream:v1:user_profile";
    }
}