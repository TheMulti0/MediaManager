namespace MediaManager.Tests
{
    internal class Counter<T>
    {
        private readonly object _lock = new object();
        private T _counter;

        public T Get()
        {
            lock (_lock)
            {
                return _counter;
            }
        }

        public void Set(T value)
        {
            lock (_lock)
            {
                _counter = value;
            }
        }
    }
}