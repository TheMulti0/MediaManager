using System.Threading.Tasks;
using MediaManager.Api;
using Xunit;

namespace MediaManager.Twitter.Tests
{
    public class TwitterLikeTests : TwitterTestBase
    {
        [Fact]
        public async void TestPostLike()
        {
            await Twitter.LikeAsync(await GetPost());
            // Add like count
        }

        [Fact]
        public async void TestPostUnlike()
        {
            await Twitter.UnlikeAsync(await GetPost());
        }
        
        private async Task<Post> GetPost() => await Twitter.FindPostAsync(Config.DefaultPostId);
    }
}