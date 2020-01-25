using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Extensions;
using Tweetinvi;
using Tweetinvi.Models;

namespace MediaManager.Twitter
{
    public class TwitterMediaProvider : IMediaProvider
    {
        public TwitterMediaProvider(
            string consumerKey,
            string consumerSecret,
            string userAccessToken,
            string userAccessSecret)
        {
            Auth.SetUserCredentials(
                consumerKey,
                consumerSecret,
                userAccessToken,
                userAccessSecret);
        }

        public async Task<MediaUser> GetIdentityAsync()
        {
            IAuthenticatedUser user = await UserAsync.GetAuthenticatedUser();
            
            return user.ToMediaUser();
        }

        public IAsyncEnumerable<Post> FindPostsAsync(string query) 
            => ToPostsAsync(SearchAsync.SearchTweets(query));

        public async IAsyncEnumerable<Post> FindPostsAsync(MediaUser author)
        {
            IUser user = await UserAsync.GetUserFromId(author.Id);
            IAsyncEnumerable<Post> posts = ToPostsAsync(user.GetUserTimelineAsync());
            
            await foreach (Post post in posts)
            {
                yield return post;
            }
        }

        public IAsyncEnumerable<Post> FindPostsAsync(MediaUser author, string query)
        {
            return FindPostsAsync(query)
                .Where(post => post.Author == author);
        }

        private static async IAsyncEnumerable<Post> ToPostsAsync(Task<IEnumerable<ITweet>> tweets)
        {
            IAsyncEnumerable<ITweet> result = await tweets.FlattenAsync();

            await foreach (ITweet tweet in result)
            {
                yield return new Post(tweet.Text, tweet.CreatedBy.ToMediaUser());
            }
        }
    }
}