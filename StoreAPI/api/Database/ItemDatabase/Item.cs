namespace Api.Database.ItemDatabase;

using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(ItemName))]
public class Item
{
    public required string ItemName { get; set; }

    public required double Price { get; set; }

    public required string VendorName {get; set;}

    public required string ItemPictureKey {get; set;}
}