using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Api;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
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

        public IAsyncEnumerable<IPost> FindPostsAsync(PostsSearchQuery query)
        {
            Task<IEnumerable<ITweet>> tweets = GetTweets(query, out bool authorQuery);
            IAsyncEnumerable<IPost> posts = ToPostsAsync(tweets);
            
            if (authorQuery)
            {
                posts = HandleAuthorQuery(query, posts);
            }

            return posts;
        }

        private static IAsyncEnumerable<IPost> HandleAuthorQuery(
            PostsSearchQuery query,
            IAsyncEnumerable<IPost> posts)
        {
            string? queryString = query.Query;
            if (queryString != null)
            {
                posts = posts.Where(post => post.Message.Contains(queryString));
            }

            DateTime? since = query.Since;
            if (since != null)
            {
                posts = posts.Where(post => post.CreatedAt >= since);
            }

            return posts;
        }

        private Task<IEnumerable<ITweet>> GetTweets(PostsSearchQuery query, out bool authorQuery)
        {
            authorQuery = query.Author != null;
            
            if (authorQuery)
            {
                return GetPostsByUserTimeline(
                    query.Author,
                    query.MaximumResults);
            }
            
            return GetPostsByQuery(
                query.Query,
                query.MaximumResults,
                query.Since);
        }

        private Task<IEnumerable<ITweet>> GetPostsByQuery(
            string query,
            int maximumResults,
            DateTime? since)
        {
            Task<IEnumerable<ITweet>> Search()
            {
                var parameters = new SearchTweetsParameters(query)
                {
                    MaximumNumberOfResults = maximumResults
                };

                if (since != null)
                {
                    parameters.Since = (DateTime) since;
                }

                return SearchAsync.SearchTweets(parameters);
            }

            return _executer.Execute(Search);
        }

        private async Task<IEnumerable<ITweet>> GetPostsByUserTimeline(
            IUser author,
            int maximumResults)
        {
            Tweetinvi.Models.IUser user = await _executer
                .Execute(() => UserAsync.GetUserFromId(author.Id));

            return await _executer
                .Execute(() => user.GetUserTimelineAsync(maximumResults));
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