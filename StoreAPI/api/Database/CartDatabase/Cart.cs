namespace Api.Database.CartDatabase;

using Amazon.DynamoDBv2.DataModel;

public class Cart
{
    [DynamoDBHashKey]
    public required string CustomerName { get; set; }

    [DynamoDBProperty]
    public required Dictionary<string, int> CartItems {get; set;}
}