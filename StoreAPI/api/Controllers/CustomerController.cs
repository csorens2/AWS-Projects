namespace Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Database;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Options;
using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;

public record AddItemRequest
{
    required public string JWT { get; set; }

    required public string ItemName { get; set; }
}

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICartDbContext _cartDbContext;

    private readonly ItemDbContext _itemDbContext;

    private readonly SaveConfig _saveConfig;

    public CustomerController(ICartDbContext cartContext, ItemDbContext itemContext, IOptions<CartDbOptions> options)
    {
        _cartDbContext = cartContext;
        _itemDbContext = itemContext;
        _saveConfig = new SaveConfig
        {
            OverrideTableName = options.Value.CartTableName
        };

    }

    [HttpGet]
    public async Task<IActionResult> RootGet()
    {
        Console.WriteLine("Hello World from Root Get of Customer");

        await _cartDbContext.SaveAsync(new Cart { CustomerUserNameHash = Guid.NewGuid().ToString(), }, _saveConfig);

        return Ok();
    }

    [HttpPost("AddToCart")]
    public async Task<IActionResult> AddToCart([FromForm] AddItemRequest request)
    {
        

        return Ok();
    }

    [HttpGet("GetCart")]
    public async Task<IActionResult> GetCart()
    {
        return Ok();
    }
}