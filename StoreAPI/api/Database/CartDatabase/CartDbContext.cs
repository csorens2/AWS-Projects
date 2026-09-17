

using System.Data.Common;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Options;

public class CartDbContext : ICartDbContext
{
    private readonly IDynamoDBContext _context;

    public CartDbContext(IDynamoDBContext dbContext)
    {
        _context = dbContext;
    }

    public Task SaveAsync<T>(T value, SaveConfig saveConfig)
    {
        return _context.SaveAsync(value, saveConfig);
    }

    public Task<T> LoadAsync<T>(object hashKey)
    {
        return _context.LoadAsync<T>(hashKey);
    }
}