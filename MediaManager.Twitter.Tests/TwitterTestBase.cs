using System.IO;
using System.Text.Json;

namespace MediaManager.Twitter.Tests
{
    public class TwitterTestBase
    {
        protected readonly TwitterProvider Twitter;
        protected readonly Configuration Config;

        protected TwitterTestBase()
        {
            Config = JsonSerializer.Deserialize<Configuration>(
                File.ReadAllText("../../../appsettings.json"));
            
            var twitterConfig = Config.Twitter;
            
            Twitter = new TwitterProvider(
                twitterConfig?.ConsumerKey,
                twitterConfig?.ConsumerSecret,
                twitterConfig?.UserAccessToken,
                twitterConfig?.UserAccessSecret);
        }
    }
}