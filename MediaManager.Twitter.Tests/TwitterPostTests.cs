using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Api;
using Xunit;

namespace MediaManager.Twitter.Tests
{
    public class TwitterPostTests : TwitterTestBase
    {
        [Fact]
        public async void TestFindPostById()
        {
            IPost post = await Twitter.FindPostAsync(Config.DefaultPostId);
            Assert.Equal(Config.DefaultPostId, post.Id);
        }
        
        [Fact]
        public async void TestFindPostsByQuery()
        {
            IAsyncEnumerable<IPost> posts = TestFindPosts(new PostsSearchQuery(Config.DefaultPostQuery));
            await CheckPostsQuery(posts);
        }
        
        [Fact]
        public async void TestFindPostsByUser()
        {
            IUser user = await Twitter.GetUserAsync(Config.DefaultUserId);
            IAsyncEnumerable<IPost> posts = TestFindPosts(new PostsSearchQuery(user));
            await CheckPostsAuthor(posts, user);
        }

        [Fact]
        public async void TestFindPostsByQueryAndUser()
        {
            IUser user = await Twitter.GetUserAsync(Config.DefaultUserId);
            IAsyncEnumerable<IPost> posts = TestFindPosts(new PostsSearchQuery(Config.DefaultPostQuery, user));
            await CheckPostsQuery(posts);
            await CheckPostsAuthor(posts, user);
        }
        
        private async IAsyncEnumerable<IPost> TestFindPosts(PostsSearchQuery postsSearchQuery)
        {
            InitializePostQuery(postsSearchQuery);

            IAsyncEnumerable<IPost> posts = Twitter.FindPostsAsync(postsSearchQuery);

            await CheckAnyPosts(posts);
            await CheckPosts(postsSearchQuery, posts);

            await foreach (IPost post in posts)
            {
                yield return post;
            }
        }

        private static void InitializePostQuery(PostsSearchQuery query)
        {
            query.MaximumResults = 2;
            query.Since = new DateTime(2018, 2, 10);
        }

        private static async Task CheckAnyPosts(IAsyncEnumerable<IPost> posts)
        {
            Assert.NotNull(posts);
            Assert.False(await posts.IsEmptyAsync());
        }
        private async Task CheckPostsQuery(IAsyncEnumerable<IPost> posts)
        {
            Assert.True(
                await posts
                    .AnyAsync(post => post.Message.Contains(Config.DefaultPostQuery)));
        }

        private async Task CheckPostsAuthor(IAsyncEnumerable<IPost> posts, IUser author)
        {
            Assert.True(
                await posts
                    .AllAsync(post => post.Author.Id == author.Id));
        }

        private static async Task CheckPosts(PostsSearchQuery postsSearchQuery, IAsyncEnumerable<IPost> posts)
        {
            Assert.True(postsSearchQuery.MaximumResults >= await posts.CountAsync());
            Assert.True(await posts.AllAsync(post => post.CreatedAt >= postsSearchQuery.Since));
        }


        [Fact]
        public async void TestPostAndDelete()
        {
            IPost post = await Twitter.PostAsync(Config.DefaultPostDescription);
            
            async Task Delete() => await Twitter.DeleteAsync(post);

            await Delete();
            await Assert.ThrowsAsync<OperationCanceledException>(Delete);
        }
    }
}