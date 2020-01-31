using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Api;

namespace MediaManager
{
    public class ProvidersOperator
    {
        public ConcurrentBag<ISocialMediaProvider> Providers { get; }

        public ProvidersOperator(ConcurrentBag<ISocialMediaProvider> providers)
        {
            Providers = providers;
        }

        public void OperateOnAll(Func<ISocialMediaProvider, Task> operation)
        {
            Providers.AsParallel().ForAll(async provider => await operation(provider));
        }
    }
}