using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MediaManager.Api;

namespace MediaManager.Tests
{
    public class MockPostsChecker : IPostsChecker
    {
        private readonly TimeSpan _delay;
        private readonly Subject<Unit> _onCheck;

        public List<IUser> WatchedUsers { get; }
        
        public IObservable<Unit> OnCheck => _onCheck;

        public MockPostsChecker(TimeSpan delay = default)
        {
            _delay = delay;
            _onCheck = new Subject<Unit>();
            WatchedUsers = new List<IUser>();
        }
        
        public Task CheckAllUsersAsync(DateTime postsSince)
        {
            _onCheck.OnNext(Unit.Default);
            return Task.Delay(_delay);
        }
    }
}