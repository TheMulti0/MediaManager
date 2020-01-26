using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Api;
using MediaManager.Extensions;
using Tweetinvi;
using Tweetinvi.Models;
using User = MediaManager.Api.User;

namespace MediaManager.Twitter
{
    public class Twitter : ISocialMediaProvider
    {
        private readonly TwitterExecuter _executer;

        public Twitter(
            string consumerKey,
            string consumerSecret,
            string userAccessToken,
            string userAccessSecret)
        {
            ITwitterCredentials credentials = Auth.CreateCredentials(
                consumerKey,
                consumerSecret,
                userAccessToken,
                userAccessSecret);
            
            _executer = new TwitterExecuter(credentials);
        }

        public Task<User> GetIdentityAsync()
            => GetUserFromTwitter(async () => (IUser) await UserAsync.GetAuthenticatedUser());

        public Task<User> GetUserAsync(long userId)
            => GetUserFromTwitter(() => UserAsync.GetUserFromId(userId));

        public Task<User> GetUserAsync(string name)
            => GetUserFromTwitter(() => UserAsync.GetUserFromScreenName(name));

        private async Task<User> GetUserFromTwitter(Func<Task<IUser>> supplier)
        {
            IUser user = await _executer.Execute(supplier);
            return user.ToUser();
        }

        public async Task<Post> FindPostAsync(long postId)
        {
            ITweet tweet = await _executer
                .Execute(() => TweetAsync.GetTweet(postId));
            
            return tweet.ToPost();
        }

        public IAsyncEnumerable<Post> FindPostsAsync(string query)
        {
            Task<IEnumerable<ITweet>> searchTweets = _executer
                .Execute(() => SearchAsync.SearchTweets(query));
            
            return ToPostsAsync(searchTweets);
        }

        public async IAsyncEnumerable<Post> FindPostsAsync(User author)
        {
            IUser user = await _executer
                .Execute(() => UserAsync.GetUserFromId(author.Id));

            var userTimelineTask = _executer
                .Execute(() => user.GetUserTimelineAsync());
            
            IAsyncEnumerable<Post> posts = ToPostsAsync(userTimelineTask);

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
            ITweet tweet = await _executer
                .Execute(() => TweetAsync.PublishTweet(content));
            
            return tweet.ToPost();
        }

        public async Task DeleteAsync(Post post)
        {
            bool success = await _executer
                .Execute(() => TweetAsync.DestroyTweet(post.Id));
            
            if (!success)
            {
                throw new OperationCanceledException();
            }
        }

        public async Task LikeAsync(Post post)
        {
            ITweet tweet = await _executer
                .Execute(() => TweetAsync.GetTweet(post.Id));
            
            if (!tweet.Favorited)
            {
                await tweet.FavoriteAsync();
            }
        }

        public async Task UnlikeAsync(Post post)
        {
            ITweet tweet = await _executer
                .Execute(() => TweetAsync.GetTweet(post.Id));
            
            if (tweet.Favorited)
            {
                await _executer
                    .Execute(() => tweet.UnFavoriteAsync());
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