using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<FileEntity> Files => Set<FileEntity>();
        public DbSet<Folder> Folders => Set<Folder>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=.\\SQLEXPRESS;Database=NetworkStorageDb;Trusted_Connection=True;TrustServerCertificate=True;"
                );
            }
        }
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // USER
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // USER -> FILES
            modelBuilder.Entity<User>()
                .HasMany(u => u.Files)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // USER -> FOLDERS
            modelBuilder.Entity<User>()
                .HasMany(u => u.Folders)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // FOLDER TREE
            modelBuilder.Entity<Folder>()
                .HasOne(f => f.ParentFolder)
                .WithMany(f => f.Children)
                .HasForeignKey(f => f.ParentFolderId)
                .OnDelete(DeleteBehavior.NoAction);

            // FILE -> FOLDER
            modelBuilder.Entity<FileEntity>()
                .HasOne(f => f.Folder)
                .WithMany(f => f.Files)
                .HasForeignKey(f => f.FolderId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
