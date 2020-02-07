using System;
using System.Threading.Tasks;

namespace MediaManager
{
    public class MediaManager : IMediaManager
    {
        private readonly TimeSpan _watchInterval;
        
        public IUserPostsChecker PostsChecker { get; }
        
        public IPostOperationValidator Validator { get; }
        
        public IProvidersOperator Operator { get; }
        
        public MediaManager(
            TimeSpan watchInterval,
            IUserPostsChecker postsChecker,
            IPostOperationValidator validator,
            IProvidersOperator @operator)
        {
            _watchInterval = watchInterval;
            PostsChecker = postsChecker;
            Validator = validator;
            Operator = @operator;
        }
        
        public void BeginUserPostWatch()
        {
            async Task RepeatWatch()
            {
                DateTime start = DateTime.Now;

                var index = 0;
                
                while (true)
                {
                    DateTime now = DateTime.Now;
                    TimeSpan beforeSleep = 
                        index != 0 
                            ? now - start 
                            : TimeSpan.Zero;
                    await DelayUntilStart(index, _watchInterval, beforeSleep);

                    index++;
                    await PostsChecker.CheckAllUsersAsync(now);
                }
            }

            Task.Run(RepeatWatch);
        }

        private static async Task DelayUntilStart(
            int index,
            TimeSpan maxInterval,
            TimeSpan now)
        {
            TimeSpan expectedStart = maxInterval * index;
            TimeSpan delay = expectedStart - now;
            
            if (delay.TotalMilliseconds > 0)
            {
                await Task.Delay(delay);
            }
        }
    }
}