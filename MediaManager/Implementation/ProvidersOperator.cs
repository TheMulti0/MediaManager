using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaManager.Api;

namespace MediaManager
{
    public class ProvidersOperator : IProvidersOperator
    {
        public ConcurrentBag<ISocialMediaProvider> Providers { get; }

        public ProvidersOperator()
        {
            Providers = new ConcurrentBag<ISocialMediaProvider>();
        }

        public Task OperateOnAllAsync(Func<ISocialMediaProvider, Task> operation)
        {
            try
            {
                ParallelQuery<Task> operations =  Providers
                    .AsParallel()
                    .Select(operation);

                return Task.WhenAll(operations);
            }
            catch (Exception)
            {
                return Task.CompletedTask;
            }
        }
    }
}