using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;
using MediaManager.Api;

namespace MediaManager.Tests
{
    public class MockUserPostsChecker : IUserPostsChecker
    {
        private readonly Subject<Unit> _onCheck;

        public IEnumerable<IUser> WatchedUsers { get; }
        
        public IObservable<Unit> OnCheck => _onCheck;

        public MockUserPostsChecker()
        {
            _onCheck = new Subject<Unit>();
            WatchedUsers = new List<IUser>();
        }
        
        public void CheckAllUsers() 
            => _onCheck.OnNext(Unit.Default);
    }
}