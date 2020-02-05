using MediaManager.Api;
using Microsoft.AspNetCore.Identity;

namespace MediaManager.Web.Models
{
    public sealed class ApplicationUser : IdentityUser<long>
    {
        public ApplicationUser()
        {
        }

        public ApplicationUser(IUser user)
        {
            Id = user.Id;
            
            UserName = user.Name;
        }
    }
}