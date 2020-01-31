using System;
using System.Threading.Tasks;
using MediaManager.Api;
using Xunit;

namespace MediaManager.Tests
{
    public class UserPostsCheckerTests
    {
        private readonly Counter<int> _onHasUserOperated = new Counter<int>();
        private readonly Counter<int> _onUserOperated = new Counter<int>();
        private readonly Counter<int> _onOperate = new Counter<int>();
        
        [Fact]
        public async Task Test()
        {
            var watchedUsers = new [] { new MockUser() };

            var validator = new MockPostOperationValidator();
            validator.OnHasUserOperated.Subscribe(OnHasUserOperated);
            validator.OnUserOperated.Subscribe(OnUserOperated);
            
            var @operator = new MockProvidersOperator();
            @operator.OnOperate.Subscribe(OnOperate);

            IUserPostsChecker checker = new UserPostsChecker(
                watchedUsers,
                validator, 
                @operator);
            
            checker.CheckAllUsers();

            await Task.Delay(TimeSpan.FromMilliseconds(15));

            int usersCount = watchedUsers.Length;
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