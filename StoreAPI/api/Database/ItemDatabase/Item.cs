
using Microsoft.EntityFrameworkCore;

namespace Api.Database;

[PrimaryKey(nameof(ItemName))]
public class Item
{
    public required string ItemName { get; set; }

    public double Price { get; set; }

    public string VendorName {get; set;}

    public string ItemPictureKey {get; set;}
}