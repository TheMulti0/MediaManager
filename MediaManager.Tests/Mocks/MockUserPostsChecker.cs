using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MediaManager.Api;

namespace MediaManager.Tests
{
    public class MockUserPostsChecker : IUserPostsChecker
    {
        private readonly TimeSpan _delay;
        private readonly Subject<Unit> _onCheck;

        public List<IUser> WatchedUsers { get; }
        
        public IObservable<Unit> OnCheck => _onCheck;

        public MockUserPostsChecker(TimeSpan delay = default)
        {
            _delay = delay;
            _onCheck = new Subject<Unit>();
            WatchedUsers = new List<IUser>();
        }
        
        public Task CheckAllUsersAsync()
        {
            _onCheck.OnNext(Unit.Default);
            return Task.Delay(_delay);
        }
    }
}