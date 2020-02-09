using System;

namespace MediaManager
{
    public interface IPostOperationValidator
    {
        IObservable<(long postId, long userId)> OnUserOperatedOnPost { get; }
        
        bool HasUserOperatedOnPost(long postId, long userId);

        void UserOperatedOnPost(long postId, long userId);
    }
}