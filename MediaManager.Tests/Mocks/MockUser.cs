using MediaManager.Api;

namespace MediaManager.Tests
{
    public class MockUser : IUser
    {
        public long Id { get; set; } = 0;

        public string Name { get; set; } = "Default";

        public string DisplayName { get; set; } = "Default mock user";

        public string Url { get; set; } = "";
        
        public bool Equals(IUser other) => Id == other?.Id;
    }
}