using System;

namespace MediaManager
{
    public class MediaManager : IMediaManager
    {
        public IUserPostsChecker PostsChecker { get; }
        
        public IPostOperationValidator Validator { get; }
        
        public IProvidersOperator Operator { get; }

        private readonly PeriodicWatchTask _task;

        public MediaManager(
            TimeSpan watchInterval,
            IUserPostsChecker postsChecker,
            IPostOperationValidator validator,
            IProvidersOperator @operator)
        {
            _task = new PeriodicWatchTask(watchInterval, postsChecker);

            PostsChecker = postsChecker;
            Validator = validator;
            Operator = @operator;
        }

        public void StartUserPostWatch() => _task.Start();

        public void StopUserPostWatch() => _task.Stop();
    }
}