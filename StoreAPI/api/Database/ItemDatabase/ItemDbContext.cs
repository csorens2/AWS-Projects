using Microsoft.EntityFrameworkCore;

namespace Api.Database;

public class ItemDbContext : DbContext
{
    public DbSet<Item> ItemSet { get; set; }

    public ItemDbContext(DbContextOptions<ItemDbContext> options)
        : base(options)
    {
        
    }
}