
using Microsoft.EntityFrameworkCore;

namespace Api.Database;

[PrimaryKey(nameof(Name))]
public class Item
{
    public string Name { get; set; }

    public double Price { get; set; }

    public string ItemPictureKey {get; set;}
}