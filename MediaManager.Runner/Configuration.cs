namespace MediaManager.Runner
{
    internal class Configuration
    {
        public class TwitterConfiguration
        {
            public string? ConsumerKey { get; set; }
            public string? ConsumerSecret { get; set; }
            public string? UserAccessToken { get; set; }
            public string? UserAccessSecret { get; set; }
        }
        
        public TwitterConfiguration? Twitter { get; set; }
    }
}