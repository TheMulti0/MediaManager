using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MediaManager.Api;

namespace MediaManager
{
    public interface IProvidersOperator
    {
        ConcurrentBag<ISocialMediaProvider> Providers { get; }

        void OperateOnAll(Func<ISocialMediaProvider, Task> operation);
    }
}