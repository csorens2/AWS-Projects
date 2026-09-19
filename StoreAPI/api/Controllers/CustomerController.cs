namespace Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Api.Database;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Options;
using Api.Utilities;
using Api.Database.CartDatabase;
using Api.Database.ItemDatabase;

public record AddItemRequest
{
    required public string JWT { get; set; }
    required public string ItemName { get; set; }
}

public record DeleteItemRequest
{
    required public string JWT {get; set;}
    required public string ItemName {get; set;}
}

public record GetCartRequest
{
    required public string JWT {get; set;}
}

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICartDbContext _cartDbContext;
    private readonly ItemDbContext _itemDbContext;
    private readonly SaveConfig _saveConfig;
    private readonly LoadConfig _loadConfig;
    private readonly ILoginTokenTool _loginTokenTool;

    public CustomerController(
        ICartDbContext cartContext, 
        ItemDbContext itemContext, 
        IOptions<CartDbOptions> options,
        ILoginTokenTool loginTokenTool)
    {
        _cartDbContext = cartContext;
        _itemDbContext = itemContext;
        _saveConfig = new SaveConfig
        {
            OverrideTableName = options.Value.CartTableName
        };
        _loadConfig = new LoadConfig
        {
            OverrideTableName = options.Value.CartTableName
        };
        _loginTokenTool = loginTokenTool;
    }

    [HttpPost("AddToCart")]
    public async Task<IActionResult> AddToCart([FromForm] AddItemRequest request)
    {
        var isValid = await _loginTokenTool.VerifyJWTAsync(request.JWT);
        if (!isValid)
        {
            return new ObjectResult(CommonApiProblems.InvalidToken());
        }

        var foundItem = _itemDbContext.Find<Item>(request.ItemName);
        if(foundItem == null)
        {
            return new ObjectResult(CommonApiProblems.ItemNotFound(request.ItemName));
        }

        var customerName = _loginTokenTool.GetUsername(request.JWT);
        
        var customerCart = await _cartDbContext.LoadAsync<Cart>(customerName, _loadConfig);
        customerCart ??= new Cart { CustomerName = customerName, CartItems = new Dictionary<string, int>()};

        if(!customerCart.CartItems.ContainsKey(request.ItemName))
        {
            customerCart.CartItems.Add(request.ItemName, 0);
        }
        customerCart.CartItems[request.ItemName] += 1;

        await _cartDbContext.SaveAsync(customerCart, _saveConfig);

        return Ok();
    }

    [HttpDelete("DeleteFromCart")]
    public async Task<IActionResult> DeleteFromCart([FromForm] DeleteItemRequest request)
    {
        var isValid = await _loginTokenTool.VerifyJWTAsync(request.JWT);
        if (!isValid)
        {
            return new ObjectResult(CommonApiProblems.InvalidToken());
        }

        var customerName = _loginTokenTool.GetUsername(request.JWT);
        
        var customerCart = await _cartDbContext.LoadAsync<Cart>(customerName, _loadConfig);
        if(customerCart == null)
        {
            return 
                Problem(
                    title: "Not Found",
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"Cart for '{customerName}' not found"
                );
        }

        if(!customerCart.CartItems.ContainsKey(request.ItemName))
        {
            return 
                Problem(
                    title: "Not Found",
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"Item'{request.ItemName}' not found in cart"
                );
        }

        customerCart.CartItems.Remove(request.ItemName);
        await _cartDbContext.SaveAsync(customerCart, _saveConfig);

        return Ok();
    }

    [HttpGet("GetCart")]
    public async Task<IActionResult> GetCart([FromForm] GetCartRequest request)
    {
        var isValid = await _loginTokenTool.VerifyJWTAsync(request.JWT);
        if (!isValid)
        {
            return new ObjectResult(CommonApiProblems.InvalidToken());
        }

        var customerName = _loginTokenTool.GetUsername(request.JWT);
        var customerCart = await _cartDbContext.LoadAsync<Cart>(customerName, _loadConfig);
        if(customerCart == null)
        {
            return 
                Problem(
                    title: "Not Found",
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"Cart for customer '{customerName}' not found"
                );
        }

        return Ok(customerCart.CartItems);
    }
}