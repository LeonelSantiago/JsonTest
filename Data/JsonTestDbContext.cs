using JsonTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JsonTest.Data
{
    public class JsonTestDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public string DatabasePath { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(u => u.Id).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.FirstName).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.LastName).IsRequired();

            base.OnModelCreating(modelBuilder);
        }

        public JsonTestDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DatabasePath = Path.Join(path, "JsonTest.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>  
            optionsBuilder.UseSqlite($"Data Source={DatabasePath}");
    }
}
