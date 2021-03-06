using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using Xunit;

namespace MediaManager.Tests
{
    public class PostsWatcherTests
    {
        private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(10);
        private readonly Counter<int> _counter = new Counter<int>();
        private readonly Counter<bool> _valid = new Counter<bool>();
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private TimeSpan _checkTime;

        [Fact]
        public async Task Test()
        {
            _counter.Set(1);
            _valid.Set(true);
            var checker = new MockPostsChecker(TimeSpan.FromMilliseconds(20));
            checker.OnCheck.Subscribe(OnCheck);
            
            var manager = new PostsWatcher(
                TimeSpan.FromMilliseconds(10),
                checker);

            manager.StartWatch();
            while (true)
            {
                if (_counter.Get() == 3 || !_valid.Get())
                {
                    break;
                }
                await Task.Delay(_delay);
            }
        }

        private void OnCheck(Unit unit)
        {
            TimeSpan elapsed = _stopwatch.Elapsed * _counter.Get();

            _valid.Set(_checkTime - elapsed < _delay);
            _checkTime = elapsed;
            _counter.Set(_counter.Get() + 1);
        }
    }
}