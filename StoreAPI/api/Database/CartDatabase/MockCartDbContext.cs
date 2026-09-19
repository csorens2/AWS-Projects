namespace Api.Database.CartDatabase;

using Amazon.DynamoDBv2.DataModel;

public class MockCartDbContext : ICartDbContext
{
    public Task<T> LoadAsync<T>(object hashKey, LoadConfig config)
    {
        return Task.FromResult<T>(default);
    }

    public Task SaveAsync<T>(T value, SaveConfig saveConfig)
    {
        return Task.CompletedTask;
    }
}