using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MovieStreaming.Custom.Models
{
    public partial class MovieDBContext : DbContext
    {

        public MovieDBContext() { }

        public MovieDBContext(DbContextOptions<MovieDBContext> dbContextOptions) : base(dbContextOptions) { }

        public virtual DbSet<User.User> Users { get; set; }
        public virtual DbSet<Role.Role> Roles { get; set; }
        public virtual DbSet<Movie.Movie> Movies { get; set; }
        public virtual DbSet<Complaint.Complaint> Complaints { get; set; }
        public virtual DbSet<ChangeLogs.ChangeLog> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Role.Role>(entity =>
            {

                entity.ToTable("Role");

                entity.Property(e => e.Name).HasMaxLength(50);

            });

            modelBuilder.Entity<User.User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Complaint.Complaint>(entity =>
            {
                entity.ToTable("Complaint");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<Movie.Movie>(entity =>
            {
                entity.ToTable("Movie");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);

        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
