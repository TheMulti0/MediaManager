namespace MediaManager
{
    public interface IPostOperationValidator
    {
        bool HasUserOperatedOnPost(long postId, long userId);

        void UserOperatedOnPost(long postId, long userId);
    }
}