using System;
using System.Reactive.Subjects;

namespace MediaManager.Tests
{
    public class MockPostOperationValidator : IPostOperationValidator
    {
        private readonly Subject<(long postId, long userId)> _onHasUserOperated;
        private readonly Subject<(long postId, long userId)> _onUserOperated;
        
        public IObservable<(long postId, long userId)> OnHasUserOperated => _onHasUserOperated;

        public IObservable<(long postId, long userId)> OnUserOperated => _onUserOperated;

        public MockPostOperationValidator()
        {
            _onHasUserOperated = new Subject<(long postId, long userId)>();
            _onUserOperated = new Subject<(long postId, long userId)>();
        }

        public bool HasUserOperatedOnPost(long postId, long userId)
        {
            _onHasUserOperated.OnNext((postId, userId));
            return true;
        }

        public void UserOperatedOnPost(long postId, long userId) 
            => _onUserOperated.OnNext((postId, userId));
    }
}