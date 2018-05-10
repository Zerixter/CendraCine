using cendracine.Models;
using cendracine.Properties;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cendracine.Data
{
    public class DbHandler : IdentityDbContext<UserIdentity>
    {
        public DbHandler(DbContextOptions<DbHandler> options) : base(options) { }

        public DbSet<User> DbUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlServer(Resources.ConnectionString);
        }
    }
}
