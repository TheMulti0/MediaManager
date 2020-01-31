using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MediaManager.Api;
using Xunit;

namespace MediaManager.Tests
{
    public class ProvidersOperatorTests
    {
        private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(20);
        
        private readonly object _counterLock = new object();
        private int _operationCounter;

        private int OperationCounter
        {
            get
            {
                lock (_counterLock)
                {
                    return _operationCounter;
                }
            }
            set
            {
                lock (_counterLock)
                {
                    _operationCounter = value;
                }
            }
        }

        [Fact]
        public void TestConcurrencyAppending()
        {
            var providers = new ConcurrentBag<ISocialMediaProvider>();
            Append(providers);
            
            var @operator = new ProvidersOperator(providers);
            Append(providers);
            
            @operator.OperateOnAll(Operation);
            Append(providers);
            
            Task.Delay(_delay * 2 * 2);
            Assert.Equal(2 - 1, OperationCounter);
        }

        private static void Append(ConcurrentBag<ISocialMediaProvider> providers)
            => providers.Add(new MockMediaProvider(1));

        private Task Operation(ISocialMediaProvider provider)
        {
            OperationCounter++;
            
            return Task.Delay(_delay);
        }
    }
}