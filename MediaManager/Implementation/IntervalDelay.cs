using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediaManager
{
    public class IntervalDelay
    {
        private readonly TimeSpan _interval;
        private readonly DateTime _start;
        private readonly CancellationToken _token;
        private int _index = 0;

        public IntervalDelay(TimeSpan interval, CancellationToken token)
        {
            _interval = interval;
            _start = DateTime.Now;
            _token = token;
        }

        public async Task DelayTillNext()
        {
            TimeSpan expectedNext = _interval * ++_index;
            TimeSpan now = DateTime.Now - _start;
            TimeSpan delay = expectedNext - now;
            
            if (delay.TotalMilliseconds > 0)
            {
                await Task.Delay(delay, _token);
            }
        }
    }
}