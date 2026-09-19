namespace Api.Database.ItemDatabase;

using Microsoft.EntityFrameworkCore;

public class ItemDbContext : DbContext
{
    public DbSet<Item> ItemSet { get; set; }

    public ItemDbContext(DbContextOptions<ItemDbContext> options)
        : base(options)
    {
        
    }
}