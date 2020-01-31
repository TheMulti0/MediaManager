using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Api;
using MediaManager.Extensions;
using Tweetinvi;
using Tweetinvi.Models;
using IUser = MediaManager.Api.IUser;

namespace MediaManager.Twitter
{
    public class TwitterProvider : ISocialMediaProvider
    {
        private readonly TwitterExecuter _executer;

        public TwitterProvider(
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

        public Task<IUser> GetIdentityAsync()
            => GetUserFromTwitter(async () => (Tweetinvi.Models.IUser) await UserAsync.GetAuthenticatedUser());

        public Task<IUser> GetUserAsync(long userId)
            => GetUserFromTwitter(() => UserAsync.GetUserFromId(userId));

        public Task<IUser> GetUserAsync(string name)
            => GetUserFromTwitter(() => UserAsync.GetUserFromScreenName(name));

        private async Task<IUser> GetUserFromTwitter(Func<Task<Tweetinvi.Models.IUser>> supplier)
        {
            return new TwitterUser(await _executer.Execute(supplier));
        }

        public async Task<IPost> FindPostAsync(long postId)
        {
            return new TwitterPost(await _executer
                .Execute(() => TweetAsync.GetTweet(postId)));
        }

        public IAsyncEnumerable<IPost> FindPostsAsync(string query)
        {
            Task<IEnumerable<ITweet>> searchTweets = _executer
                .Execute(() => SearchAsync.SearchTweets(query));
            
            return ToPostsAsync(searchTweets);
        }

        public async IAsyncEnumerable<IPost> FindPostsAsync(IUser author)
        {
            Tweetinvi.Models.IUser user = await _executer
                .Execute(() => UserAsync.GetUserFromId(author.Id));

            var userTimelineTask = _executer
                .Execute(() => user.GetUserTimelineAsync());
            
            IAsyncEnumerable<IPost> posts = ToPostsAsync(userTimelineTask);

            await foreach (IPost post in posts)
            {
                yield return post;
            }
        }

        public IAsyncEnumerable<IPost> FindPostsAsync(IUser author, string query)
        {
            return FindPostsAsync(query)
                .Where(post => post.Author == author);
        }

        public async Task<IPost> PostAsync(string description)
        {
            return new TwitterPost(await _executer
                .Execute(() => TweetAsync.PublishTweet(description)));
        }

        public async Task DeleteAsync(IPost post)
        {
            bool success = await _executer
                .Execute(() => TweetAsync.DestroyTweet(post.Id));
            
            if (!success)
            {
                throw new OperationCanceledException();
            }
        }

        public async Task LikeAsync(IPost post)
        {
            ITweet tweet = await _executer
                .Execute(() => TweetAsync.GetTweet(post.Id));
            
            if (!tweet.Favorited)
            {
                await tweet.FavoriteAsync();
            }
        }

        public async Task UnlikeAsync(IPost post)
        {
            ITweet tweet = await _executer
                .Execute(() => TweetAsync.GetTweet(post.Id));
            
            if (tweet.Favorited)
            {
                await _executer
                    .Execute(() => tweet.UnFavoriteAsync());
            }
        }

        private static async IAsyncEnumerable<IPost> ToPostsAsync(Task<IEnumerable<ITweet>> tweets)
        {
            IAsyncEnumerable<ITweet> result = await tweets.FlattenAsync();

            await foreach (ITweet tweet in result)
            {
                yield return new TwitterPost(tweet);
            }
        }
    }
}