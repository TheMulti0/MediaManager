using System.IO;
using System.Text.Json;

namespace MediaManager.Tests
{
    public class TwitterTestBase
    {
        protected readonly Twitter Twitter;
        protected readonly Configuration Config;

        protected TwitterTestBase()
        {
            Config = JsonSerializer.Deserialize<Configuration>(
                File.ReadAllText("../../../appsettings.json"));
            
            var twitterConfig = Config.Twitter;
            
            Twitter = new Twitter(
                twitterConfig?.ConsumerKey,
                twitterConfig?.ConsumerSecret,
                twitterConfig?.UserAccessToken,
                twitterConfig?.UserAccessSecret);
        }
    }
}