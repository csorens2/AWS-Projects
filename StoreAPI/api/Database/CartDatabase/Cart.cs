

using Amazon.DynamoDBv2.DataModel;

public class Cart
{
    [DynamoDBHashKey]
    public string CustomerUserNameHash { get; set; }
}