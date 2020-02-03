using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MediaManager.Extensions;

namespace MediaManager
{
    public class MediaManager : IMediaManager
    {
        private readonly TimeSpan _watchMaximumInterval;
        
        public IUserPostsChecker PostsChecker { get; }
        
        public IPostOperationValidator Validator { get; }
        
        public IProvidersOperator Operator { get; }
        
        public MediaManager(
            TimeSpan watchMaximumInterval,
            IUserPostsChecker postsChecker,
            IPostOperationValidator validator,
            IProvidersOperator @operator)
        {
            _watchMaximumInterval = watchMaximumInterval;
            PostsChecker = postsChecker;
            Validator = validator;
            Operator = @operator;
        }
        
        public void BeginUserPostWatch()
        {
            async Task RepeatWatch()
            {
                var stopwatch = Stopwatch.StartNew();

                var index = 0;
                while (true)
                {
                    await DelayUntilStart(index, _watchMaximumInterval, stopwatch.Elapsed);

                    index++;
                    await PostsChecker.CheckAllUsersAsync();
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