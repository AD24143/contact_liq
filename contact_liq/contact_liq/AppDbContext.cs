using Microsoft.EntityFrameworkCore;

namespace contact_liq;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=contacts.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Friends" },
            new Category { Id = 2, Name = "Family" },
            new Category { Id = 3, Name = "Work" }
        );
        
        modelBuilder.Entity<Contact>().HasData(
            new Contact { Id = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice.johnson@example.com", Age = 24, City = "Warsaw", CategoryId = 1 },
            new Contact { Id = 2, FirstName = "Brian", LastName = "Taylor", Email = "brian.taylor@example.com", Age = 31, City = "Krakow", CategoryId = 3 },
            new Contact { Id = 3, FirstName = "Clara", LastName = "Evans", Email = "clara.evans@example.com", Age = 28, City = "Warsaw", CategoryId = 2 },
            new Contact { Id = 4, FirstName = "David", LastName = "Miller", Email = "david.miller@example.com", Age = 36, City = "Gdansk", CategoryId = 3 },
            new Contact { Id = 5, FirstName = "Eva", LastName = "Nowak", Email = "eva.nowak@example.com", Age = 29, City = "Wroclaw", CategoryId = 1 }
        );
    }
}
