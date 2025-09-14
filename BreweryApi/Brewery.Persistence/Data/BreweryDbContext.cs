using Microsoft.EntityFrameworkCore;
using Brewery.Core.Entities;

public class BreweryDbContext : DbContext
{
    public BreweryDbContext(DbContextOptions<BreweryDbContext> options) : base(options) { }

    public DbSet<BreweryEntity> Breweries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BreweryEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.City);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
        });
    }
}