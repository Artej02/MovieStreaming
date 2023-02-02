using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MovieStreaming.Areas.Admin.Models.ChangeLogs;
using MovieStreaming.Areas.Admin.Models.Complaint;
using MovieStreaming.Areas.Admin.Models.Movie;
using MovieStreaming.Areas.Admin.Models.Role;
using MovieStreaming.Custom.Models.User;

namespace MovieStreaming.Custom.Models
{
    public partial class MovieDBContext : DbContext
    {

        public MovieDBContext() { }

        public MovieDBContext(DbContextOptions<MovieDBContext> dbContextOptions) : base(dbContextOptions) { }

        public virtual DbSet<User.User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Complaint> Complaints { get; set; }
        public virtual DbSet<ChangeLog> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Role>(entity =>
            {

                entity.ToTable("Role");

                entity.Property(e => e.Name).HasMaxLength(50);

            });

            modelBuilder.Entity<User.User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.ToTable("Complaint");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.ToTable("Movie");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);

        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
