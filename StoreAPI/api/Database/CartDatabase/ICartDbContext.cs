using Amazon.DynamoDBv2.DataModel;

public interface ICartDbContext
{
    Task SaveAsync<T>(T value, SaveConfig saveConfig);
}