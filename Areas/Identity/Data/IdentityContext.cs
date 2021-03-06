using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrainTickets.Areas.Identity.Data;

namespace TrainTickets.Areas.Identity.Data;

public class IdentityContext : IdentityDbContext<ApplicationUser>
{
    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>()
        .HasIndex(u => u.Email)
        .IsUnique();
        builder.Entity<ApplicationUser>()
        .HasIndex(u => u.PhoneNumber)
        .IsUnique();
        builder.Entity<ApplicationUser>()
        .HasIndex(u => new { u.FirstName, u.LastName })
        .IsUnique();
        builder.Entity<ApplicationUser>()
        .HasIndex(u => u.IIN)
        .IsUnique();
    }
}
