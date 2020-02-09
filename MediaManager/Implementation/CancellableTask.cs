using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediaManager
{
    class CancellableTask
    {
        private readonly Task _task;
        private readonly CancellationTokenSource _cts;

        public CancellableTask(Func<CancellationToken, Task> asyncMethod)
        {
            _cts = new CancellationTokenSource();
            _task = asyncMethod(_cts.Token);
        }

        public void Stop()
        {
            try
            {
                _cts.Cancel();
                _task.Wait();
            }
            catch
            {
                // ReSharper disable once EmptyGeneralCatchClause
            }
        }
    }
}