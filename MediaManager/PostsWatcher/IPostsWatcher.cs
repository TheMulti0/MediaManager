using System;

namespace MediaManager
{
    public interface IPostsWatcher
    {
        public IObservable<DateTime> OnPostsCheck { get; }

        public DateTime LastPostsCheckTime { get; }
        
        void StartWatch();
        
        void StopWatch();
    }
}