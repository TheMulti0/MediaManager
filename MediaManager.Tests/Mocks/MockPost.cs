using System;
using MediaManager.Api;

namespace MediaManager.Tests
{
    public class MockPost : IPost
    {
        public long Id { get; set; } = 0;

        public string Message { get; set; } = "This is a mock post";
        
        public IUser Author { get; set; } = new MockUser();
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Url { get; set; } = "";
        
        public bool Equals(IPost other) => Id == other?.Id;

    }
}