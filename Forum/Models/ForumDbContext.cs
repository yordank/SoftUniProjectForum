using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Forum.Models
{
  

    public class ForumDbContext : IdentityDbContext<ApplicationUser>
    {
        public ForumDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public virtual IDbSet<Post> Posts { get; set; }

        public static ForumDbContext Create()
        {
            return new ForumDbContext();
        }
    }
}