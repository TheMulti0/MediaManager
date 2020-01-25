using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediaManager.Twitter;

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
            
            IMediaProvider provider = new TwitterMediaProvider(
                twitterConfig?.ConsumerKey,
                twitterConfig?.ConsumerSecret,
                twitterConfig?.UserAccessToken,
                twitterConfig?.UserAccessSecret);

            MediaUser identity = await provider.GetIdentityAsync();

            List<Post> result = await provider
                .FindPostsAsync(identity, "a")
                .ToListAsync();
        }
        
        private static ValueTask<Configuration> ReadJsonAsync()
            => JsonSerializer.DeserializeAsync<Configuration>(new FileStream("../../../appsettings.json", FileMode.Open));
    }
}