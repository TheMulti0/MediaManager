using Xunit;

namespace MediaManager.Twitter.Tests
{
    public class TwitterUserTests : TwitterTestBase
    {
        [Fact]
        public async void TestIdentity()
        {
            Assert.NotNull(await Twitter.GetIdentityAsync());
        }

        [Fact]
        public async void TestGetUserById()
        {
            Assert.NotNull(await Twitter.GetUserAsync(Config.DefaultUserId));
        }

        [Fact]
        public async void TestGetUserByName()
        {
            Assert.NotNull(await Twitter.GetUserAsync(Config.DefaultUserName));
        }
    }
}