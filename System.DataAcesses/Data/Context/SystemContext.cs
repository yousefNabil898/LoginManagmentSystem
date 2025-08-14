using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.DataAcesses.Models;

namespace System.DataAcesses.Data.Context
{
    public class SystemContext : IdentityDbContext<Company, IdentityRole<Guid>, Guid>
    {
        public SystemContext(DbContextOptions<SystemContext> options) : base(options)
        {
        }

        public DbSet<Otp> Otps { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Otp>()
                .HasOne(o => o.Company)
                .WithMany(c => c.Otps)
                .HasForeignKey(o => o.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Company>().ToTable("Companies");

        }
    }
}
