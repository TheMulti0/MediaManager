using MediaManager.Web.Data.Entities;
using MediaManager.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MediaManager.Web.Data
{
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
    {
        public DbSet<WatchedUser>? WatchedUsers { get; set; }

        public DbSet<OperatedPost>? OperatedPosts { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
