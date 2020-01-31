using System;
using System.Reactive.Linq;

namespace MediaManager
{
    public class MediaManager : IMediaManager
    {
        public IUserPostsChecker PostsChecker { get; }
        
        public IPostOperationValidator Validator { get; }
        
        public IProvidersOperator Operator { get; }
        
        public MediaManager(
            IUserPostsChecker postsChecker,
            IPostOperationValidator validator,
            IProvidersOperator @operator)
        {
            PostsChecker = postsChecker;
            Validator = validator;
            Operator = @operator;
        }
        
        public void BeginUserPostWatch(TimeSpan interval)
        {
            Observable
                .Interval(interval)
                .Subscribe(_ => PostsChecker.CheckAllUsers());
        }
    }
}