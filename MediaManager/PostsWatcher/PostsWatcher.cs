using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace MediaManager
{
    public class PostsWatcher : IPostsWatcher
    {
        private readonly Subject<DateTime> _onPostsCheck = new Subject<DateTime>();
        private readonly TimeSpan _interval;
        private readonly IPostsChecker _postsChecker;
        private readonly object _taskLock = new object();
        private readonly object _checkTimeLock = new object();

        private IntervalDelay? _delay;
        private CancellableTask? _task;

        public IObservable<DateTime> OnPostsCheck => _onPostsCheck;

        public DateTime LastPostsCheckTime { get; private set; } = DateTime.Now;

        public PostsWatcher(
            TimeSpan interval,
            IPostsChecker postsChecker)
        {
            _interval = interval;
            _postsChecker = postsChecker;
        }

        public void StartWatch()
        {
            lock (_taskLock)
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
            lock (_taskLock)
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

                await _postsChecker.CheckAllUsersAsync(LastPostsCheckTime);

                DateTime checkTime = DateTime.Now;
                _onPostsCheck.OnNext(checkTime);
                
                lock (_checkTimeLock)
                {
                    LastPostsCheckTime = checkTime;
                }
                
                await _delay.DelayTillNext();
            }
        }
    }
}