namespace Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Options;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICartDbContext _context;

    private readonly SaveConfig _saveConfig;

    public CustomerController(ICartDbContext dbContext, IOptions<CartDbOptions> options)
    {
        _context = dbContext;

        _saveConfig = new SaveConfig
        {
            OverrideTableName = options.Value.CartTableName
        };
    }

    [HttpGet]
    public async Task<IActionResult> RootGet()
    {
        Console.WriteLine("Hello World from Root Get of Customer");

        await _context.SaveAsync(new Cart { CustomerUserNameHash = Guid.NewGuid().ToString(), }, _saveConfig);

        return Ok();
    }
}