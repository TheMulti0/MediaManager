using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Extensions;
using Tweetinvi;
using Tweetinvi.Models;

namespace MediaManager
{
    public class Twitter : ISocialMedia
    {
        public Twitter(
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

        public async Task<User> GetIdentityAsync()
        {
            IAuthenticatedUser user = await UserAsync.GetAuthenticatedUser();
            return user.ToMediaUser();
        }

        public IAsyncEnumerable<Post> FindPostsAsync(string query)
        {
            Task<IEnumerable<ITweet>> searchTweets = SearchAsync.SearchTweets(query);
            return ToPostsAsync(searchTweets);
        }

        public async IAsyncEnumerable<Post> FindPostsAsync(User author)
        {
            IUser user = await UserAsync.GetUserFromId(author.Id);
            IAsyncEnumerable<Post> posts = ToPostsAsync(user.GetUserTimelineAsync());

            await foreach (Post post in posts)
            {
                yield return post;
            }
        }

        public IAsyncEnumerable<Post> FindPostsAsync(User author, string query)
        {
            return FindPostsAsync(query)
                .Where(post => post.Author == author);
        }

        public async Task<Post> PostAsync(string content)
        {
            ITweet tweet = await TweetAsync.PublishTweet(content);
            return tweet.ToPost();
        }

        public async Task DeleteAsync(Post post)
        {
            bool success = await TweetAsync.DestroyTweet(post.Id);
            if (!success)
            {
                throw new OperationCanceledException();
            }
        }

        public async Task LikeAsync(Post post)
        {
            ITweet tweet = await TweetAsync.GetTweet(post.Id);
            if (!tweet.Favorited)
            {
                await tweet.FavoriteAsync();
            }
        }

        public async Task UnlikeAsync(Post post)
        {
            ITweet tweet = await TweetAsync.GetTweet(post.Id);
            if (tweet.Favorited)
            {
                await tweet.UnFavoriteAsync();
            }
        }

        private static async IAsyncEnumerable<Post> ToPostsAsync(Task<IEnumerable<ITweet>> tweets)
        {
            IAsyncEnumerable<ITweet> result = await tweets.FlattenAsync();

            await foreach (ITweet tweet in result)
            {
                yield return tweet.ToPost();
            }
        }
    }
}