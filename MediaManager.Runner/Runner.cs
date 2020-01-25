using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MediaManager.Runner
{
    internal class Runner
    {
        public static Task Main(string[] args)
            => new Runner().RunAsync();

        private async Task RunAsync()
        {
            Configuration config = await ReadJsonAsync();
            Configuration.TwitterConfiguration? twitterConfig = config.Twitter;
            
            ISocialMedia provider = new Twitter(
                twitterConfig?.ConsumerKey,
                twitterConfig?.ConsumerSecret,
                twitterConfig?.UserAccessToken,
                twitterConfig?.UserAccessSecret);
        }
        
        private static ValueTask<Configuration> ReadJsonAsync()
            => JsonSerializer.DeserializeAsync<Configuration>(
                new FileStream("../../../appsettings.json", FileMode.Open));
    }
}