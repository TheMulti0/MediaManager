namespace MediaManager
{
    public interface IMediaManager
    {
        IUserPostsChecker PostsChecker { get; }

        IPostOperationValidator Validator { get; }

        IProvidersOperator Operator { get; }
        
        void StartUserPostWatch();
        void StopUserPostWatch();
    }
}