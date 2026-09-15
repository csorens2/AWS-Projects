

using Amazon.DynamoDBv2.DataModel;

public class MockCartDbContext : ICartDbContext
{
    public Task SaveAsync<T>(T value, SaveConfig saveConfig)
    {
        return Task.CompletedTask;
    }
}