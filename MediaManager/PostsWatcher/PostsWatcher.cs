using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediaManager
{
    public class PostsWatcher : IPostsWatcher
    {
        private readonly TimeSpan _interval;
        private readonly IPostsChecker _postsChecker;
        private readonly object _lock = new object();

        private IntervalDelay? _delay;
        private CancellableTask? _task;
        
        public PostsWatcher(
            TimeSpan interval,
            IPostsChecker postsChecker)
        {
            _interval = interval;
            _postsChecker = postsChecker;
        }

        public void StartWatch()
        {
            lock (_lock)
            {
                Stop();

                _task = new CancellableTask(
                    token => Task.Run(
                        () => RepeatWatch(token),
                        token));
            }
        }
    
        public void StopWatch()
        {
            lock (_lock)
            {
                Stop();
            }
        }

        private void Stop()
        {
            _task?.Stop();
            _task = null;
        }

        private async Task RepeatWatch(CancellationToken token)
        {
            try
            {
                await RepeatWatchMethod(token);
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
        }

        private async Task RepeatWatchMethod(CancellationToken token)
        {
            _delay = new IntervalDelay(_interval, token);

            while(true)
            {
                token.ThrowIfCancellationRequested();

                await _postsChecker.CheckAllUsersAsync(DateTime.Now);
                await _delay.DelayTillNext();
            }
        }
    }
}