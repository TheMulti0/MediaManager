using MediaManager.Api;

namespace MediaManager.Web.Models
{
    public class WatchedUser : IUser
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string DisplayName { get; set; }
        
        public string Url { get; set; }
        
        public bool Equals(IUser other) => Id == other?.Id;
    }
}