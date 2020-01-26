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
    }
}