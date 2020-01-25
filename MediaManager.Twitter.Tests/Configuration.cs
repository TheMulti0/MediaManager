namespace MediaManager.Tests
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

        public long Id { get; set; }
        
        public string Query { get; set; }
        
        public string Description { get; set; }
    }
}