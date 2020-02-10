using MediaManager.Api;
using Microsoft.AspNetCore.Identity;

namespace MediaManager.Web.Data.Entities
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