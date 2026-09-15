using Amazon.S3;
using Amazon.S3.Model;
using Api.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers;

public record NewItemRequest()
{
    required public string ItemName { get; set; }
    required public double ItemCost { get; set; }
    required public string JWT { get; set; }
    required public IFormFile ItemPicture { get; set; }
}

[ApiController]
[Route("[controller]")]
public class VendorController : ControllerBase
{
    private readonly ItemDbContext _context;
    private readonly ApiOptions _options;
    private readonly IAmazonS3 _s3Client;

    public VendorController(ItemDbContext dbContext, IOptions<ApiOptions> settings, IAmazonS3 s3Client)
    {
        _context = dbContext;
        _options = settings.Value;
        _s3Client = s3Client;
    }

    [HttpGet]
    public IActionResult RootGet()
    {
        Console.WriteLine("Hello World from Vendor Root");
        return Ok();
    }
    
    [HttpPost("NewItem")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> PostNewItem([FromForm] NewItemRequest request)
    {
        var isValid = await LoginTokenValidator.VerifyJWTAsync(request.JWT, _options.Region, _options.UserPoolId);
        if (!isValid)
        {
            Console.WriteLine("Invalid Login Token");
            return Unauthorized();
        }

        var cognitoGroups = LoginTokenValidator.GetCognitoGroups(request.JWT);
        if (!cognitoGroups.Contains(_options.VendorGroupName))
        {
            Console.WriteLine("Must be a vendor account to add items");
            return Forbid();
        }

        var file = request.ItemPicture;

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        string itemKey = $"uploads/{Guid.NewGuid()}_{file.FileName}";
        using var stream = file.OpenReadStream();
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _options.ItemPicturesBucketName,
            Key = itemKey,
            InputStream = stream,
        };
        await _s3Client.PutObjectAsync(putObjectRequest);

        _context.Add(
            new Item 
            { 
                Name = request.ItemName, 
                Price = request.ItemCost, 
                ItemPictureKey = itemKey 
            });
        _context.SaveChanges();

        return Ok();
    }
}