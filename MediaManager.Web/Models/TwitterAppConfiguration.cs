using Microsoft.Extensions.Configuration;

namespace MediaManager.Web.Models
{
    public class TwitterAppConfiguration
    {
        public string ConsumerKey { get; }

        public string ConsumerSecret { get; }
        
        public TwitterAppConfiguration(IConfiguration configuration)
        {
            ConsumerKey = configuration["Authentication:Twitter:ConsumerKey"];
            ConsumerSecret = configuration["Authentication:Twitter:ConsumerSecret"];
        }
    }
}