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
    public class DbHandler : DbContext
    {
        public DbHandler() { }
        public DbHandler(DbContextOptions<DbHandler> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<BillboardMovieRegister> BillboardMovieRegisters { get; set; }
        public DbSet<Billboard> Billboards { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Projection> Projections { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Seat> Seats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlServer(Resources.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            #region Movie
            builder.Entity<Movie>()
                .HasMany(x => x.BillboardMovieRegister)
                .WithOne(x => x.Movie);
            builder.Entity<Movie>()
                .HasMany(x => x.Categories);
            builder.Entity<Movie>()
                .HasMany(x => x.Projections)
                .WithOne(x => x.Movie);
            builder.Entity<Movie>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.Movies);
            #endregion
            #region Billboard
            builder.Entity<Billboard>()
                .HasKey(x => x.Id);
            builder.Entity<Billboard>()
                .HasMany(x => x.BillboardMovieRegister)
                .WithOne(x => x.Billboard);
            #endregion
            #region BillboardMovieRegister
            builder.Entity<BillboardMovieRegister>()
                .HasOne(x => x.Billboard);
            builder.Entity<BillboardMovieRegister>()
                .HasOne(x => x.Movie);
            #endregion
            #region User
            builder.Entity<User>()
                .HasMany(x => x.Billboards)
                .WithOne(x => x.Owner);
            builder.Entity<User>()
                .HasMany(x => x.Reservations)
                .WithOne(x => x.Owner);
            builder.Entity<User>()
                .HasMany(x => x.Movies)
                .WithOne(x => x.Owner);
            #endregion
            #region Category
            builder.Entity<Category>()
                .HasMany(x => x.Movies);
            #endregion
            #region Projection
            builder.Entity<Projection>()
                .HasOne(x => x.Movie)
                .WithMany(x => x.Projections);
            builder.Entity<Projection>()
                .HasOne(x => x.Theater)
                .WithMany(x => x.Projections);
            builder.Entity<Projection>()
                .HasMany(x => x.Reservations)
                .WithOne(x => x.Projection);
            #endregion
            #region Theater
            builder.Entity<Theater>()
                .HasMany(x => x.Projections)
                .WithOne(x => x.Theater);
            builder.Entity<Theater>()
                .HasMany(x => x.Seats)
                .WithOne(x => x.Theater);
            #endregion
            #region Reservation
            builder.Entity<Reservation>()
                .HasOne(x => x.Projection)
                .WithMany(x => x.Reservations);
            builder.Entity<Reservation>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.Reservations);
            builder.Entity<Reservation>()
                .HasOne(x => x.Seat)
                .WithMany(x => x.Reservations);
            #endregion
            #region Seat
            builder.Entity<Seat>()
                .HasOne(x => x.Theater)
                .WithMany(x => x.Seats);
            builder.Entity<Seat>()
                .HasMany(x => x.Reservations)
                .WithOne(x => x.Seat);
            #endregion
        }
    }
}
