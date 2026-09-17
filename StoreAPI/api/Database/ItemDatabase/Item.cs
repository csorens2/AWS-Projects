
using Microsoft.EntityFrameworkCore;

namespace Api.Database;

[PrimaryKey(nameof(ItemName))]
public class Item
{
    public required string ItemName { get; set; }

    public required double Price { get; set; }

    public required string VendorUserName {get; set;}

    public required string ItemPictureKey {get; set;}
}