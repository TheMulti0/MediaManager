using System.Diagnostics.CodeAnalysis;

namespace MediaManager.Twitter.Tests
{
    public class Configuration
    {
        public class TwitterConfiguration
        {
            public string? ConsumerKey { get; set; }
            public string? ConsumerSecret { get; set; }
            public string? UserAccessToken { get; set; }
            public string? UserAccessSecret { get; set; }
        }
        
        public TwitterConfiguration? Twitter { get; set; }

        public long DefaultPostId { get; set; }

        public string? DefaultPostQuery { get; set; }
        
        public string? DefaultPostDescription { get; set; }

        public long DefaultUserId { get; set; }

        public string? DefaultUserName { get; set; }
    }
}