using System.Threading.Tasks;
using Xunit;

namespace MediaManager.Tests
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
        
        private async Task<Post> GetPost() => await Twitter.FindPostAsync(Config.Id);
    }
}