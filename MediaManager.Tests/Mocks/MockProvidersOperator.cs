using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MediaManager.Api;

namespace MediaManager.Tests
{
    public class MockProvidersOperator : IProvidersOperator
    {
        private readonly Subject<Func<ISocialMediaProvider, Task>> _onOperate;

        public ConcurrentBag<ISocialMediaProvider> Providers { get; }

        public IObservable<Func<ISocialMediaProvider, Task>> OnOperate => _onOperate;

        public MockProvidersOperator()
        {
            _onOperate = new Subject<Func<ISocialMediaProvider, Task>>();

            Providers = new ConcurrentBag<ISocialMediaProvider>();
        }

        public void OperateOnAll(Func<ISocialMediaProvider, Task> operation)
        {
            _onOperate.OnNext(operation);
        }
    }
}