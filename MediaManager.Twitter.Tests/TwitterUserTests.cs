using Xunit;

namespace MediaManager.Tests
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