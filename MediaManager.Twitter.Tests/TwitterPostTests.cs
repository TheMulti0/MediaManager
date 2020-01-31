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
            IAsyncEnumerable<IPost> posts = Twitter.FindPostsAsync(Config.DefaultPostQuery);
            Assert.False(await posts.IsEmptyAsync());
        }

        [Fact]
        public async void TestFindPostsByUser()
        {
            IUser user = await Twitter.GetUserAsync(Config.DefaultUserId);
            IAsyncEnumerable<IPost> posts = Twitter.FindPostsAsync(user);
            Assert.False(await posts.IsEmptyAsync());
        }

        [Fact]
        public async void TestFindPostsByQueryAndUser()
        {
            IUser user = await Twitter.GetUserAsync(Config.DefaultUserId);
            IAsyncEnumerable<IPost> posts = Twitter.FindPostsAsync(user, "C");
            Assert.False(await posts.IsEmptyAsync());
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