using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediaManager
{
    internal class PeriodicWatchTask
    {
        private readonly TimeSpan _interval;
        private readonly IUserPostsChecker _postsChecker;
        private readonly object _lock = new object();

        private IntervalDelay? _delay;
        private CancellableTask? _task;

        
        public PeriodicWatchTask(
            TimeSpan interval,
            IUserPostsChecker postsChecker)
        {
            _interval = interval;
            _postsChecker = postsChecker;
        }

        public void Start()
        {
            lock (_lock)
            {
                _Stop();

                _task = new CancellableTask(
                    token => Task.Run(
                        () => RepeatWatch(token),
                        token));
            }
        }
    
        public void Stop()
        {
            lock (_lock)
            {
                _Stop();
            }
        }

        private void _Stop()
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