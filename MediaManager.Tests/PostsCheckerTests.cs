using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Api;
using Xunit;

namespace MediaManager.Tests
{
    public class PostsCheckerTests
    {
        private readonly Counter<int> _onHasUserOperated = new Counter<int>();
        private readonly Counter<int> _onUserOperated = new Counter<int>();
        private readonly Counter<int> _onOperate = new Counter<int>();
        
        [Fact]
        public async Task TestWithoutProviders()
        {
            await Test(new List<ISocialMediaProvider>());
        }
        
        [Fact]
        public async Task TestWithOneProvider()
        {
            await Test(new []{ new MockMediaProvider(1) });
        }

        private async Task Test(IEnumerable<ISocialMediaProvider> providers)
        {
            var watchedUsers = new List<MockUser>()
            {
                new MockUser()
            };

            var validator = new MockPostOperationValidator();
            validator.OnHasUserOperated.Subscribe(OnHasUserOperated);
            validator.OnUserOperatedOnPost.Subscribe(OnUserOperated);

            var @operator = new MockProvidersOperator();
            foreach (ISocialMediaProvider provider in providers)
            {
                @operator.Providers.Add(provider);
            }
            @operator.OnOperate.Subscribe(OnOperate);

            IPostsChecker checker = new PostsChecker(
                validator,
                @operator);
            checker.WatchedUsers.AddRange(watchedUsers);

            await checker.CheckAllUsersAsync(DateTime.Now);

            await Task.Delay(TimeSpan.FromMilliseconds(15));

            int usersCount = watchedUsers.Count;
            int callCount = usersCount * @operator.Providers.Count;

            Assert.Equal(callCount, _onHasUserOperated.Get());
            Assert.Equal(callCount, _onUserOperated.Get());
            Assert.Equal(usersCount, _onOperate.Get());
        }

        private void OnHasUserOperated((long postId, long userId) arg)
        {
            _onHasUserOperated.Set(_onHasUserOperated.Get() + 1);
        }

        private void OnUserOperated((long postId, long userId) arg)
        {
            _onUserOperated.Set(_onUserOperated.Get() + 1);
        }
        
        private void OnOperate(Func<ISocialMediaProvider, Task> arg)
        {
            _onOperate.Set(_onOperate.Get() + 1);
        }
    }
}