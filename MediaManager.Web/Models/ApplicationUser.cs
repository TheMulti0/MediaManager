using MediaManager.Api;
using Microsoft.AspNetCore.Identity;

namespace MediaManager.Web.Models
{
    public sealed class ApplicationUser : IdentityUser<long>
    {
        public long TwitterId { get; }

        public ApplicationUser()
        {
        }

        public ApplicationUser(IUser user)
        {
            TwitterId = user.Id;
            
            UserName = user.Name;
        }

        public bool Equals(IUser other) => TwitterId == other?.Id;
    }
}