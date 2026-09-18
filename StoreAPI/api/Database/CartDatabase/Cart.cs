

using Amazon.DynamoDBv2.DataModel;

public class Cart
{
    [DynamoDBHashKey]
    public string CustomerName { get; set; }

    [DynamoDBProperty]
    public List<string> CartItems {get; set;}
}