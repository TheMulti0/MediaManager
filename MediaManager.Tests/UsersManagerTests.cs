using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Api;
using Xunit;

namespace MediaManager.Tests
{
    public class UsersManagerTests
    {
        private const int TotalPostCount = 5;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMilliseconds(20);
        private readonly object _likeCountLock = new object();
        private int _likeCount = 0;

        [Fact]
        public async Task Test1()
        {
            var provider = new MockMediaProvider(5);
            provider.OnPostLike.Subscribe(OnLike);
            await StartUsersManager(provider);

            TimeSpan elapsedTime = TimeSpan.Zero;
            for (var i = 0; i <= TotalPostCount; i++)
            {
                if (IsOperationComplete())
                {
                    return;
                }
                if (elapsedTime > _checkInterval * TotalPostCount)
                {
                    throw new Exception("Elapsed time is longer than expected");
                }
                await Task.Delay(_checkInterval);
                elapsedTime += _checkInterval;
            }
        }

        private bool IsOperationComplete()
        {
            lock (_likeCountLock)
            {
                if (_likeCount == TotalPostCount)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<UsersManager> StartUsersManager(ISocialMediaProvider provider)
        {
            IUser user = await provider.GetIdentityAsync();
            return new UsersManager(new List<IUser>
            {
                new MockUser
                {
                    Name = "Watched"
                }
            },
                new Dictionary<IUser, ISocialMediaProvider>
                {
                    {user, provider}
                }, _checkInterval);
        }

        private void OnLike(IPost post)
        {
            lock (_likeCountLock)
            {
                _likeCount++;
            }
        }
    }
}