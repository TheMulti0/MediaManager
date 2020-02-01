using System;

namespace MediaManager
{
    public interface IMediaManager
    {
        IUserPostsChecker PostsChecker { get; }

        IPostOperationValidator Validator { get; }

        IProvidersOperator Operator { get; }
        
        void BeginUserPostWatch(TimeSpan maxInterval);
    }
}