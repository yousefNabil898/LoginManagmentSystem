using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.DataAcesses.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Data.Context
{
    public class SystemContext : DbContext
    {
        public SystemContext(DbContextOptions<SystemContext> options) :base(options)
        {
            
        }
        public DbSet<Company> Companies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Company>().HasIndex(c => c.Email).IsUnique();
        }

    }
}
