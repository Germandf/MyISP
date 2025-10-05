using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyISP.Identity;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}

public class ApplicationUser : IdentityUser
{
    public Guid CustomerId { get; set; } = Guid.Parse("11111111-1111-1111-1111-111111111111");
}
