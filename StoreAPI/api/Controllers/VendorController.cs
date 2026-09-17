namespace Api.Controllers;

using Amazon.S3;
using Amazon.S3.Model;
using Api.Utilities;
using Api.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public record NewItemRequest
{
    required public string ItemName { get; set; }
    required public double ItemCost { get; set; }
    required public string JWT { get; set; }
    required public IFormFile ItemPicture { get; set; }
}

public record DeleteRequest
{
    required public string ItemName {get; set;}
    required public string JWT {get; set;}
}

[ApiController]
[Route("[controller]")]
public class VendorController : ControllerBase
{
    private readonly ItemDbContext _context;
    private readonly ApiOptions _options;
    private readonly IAmazonS3 _s3Client;
    private readonly ILoginTokenTool _tokenTool;

    public VendorController(
        ItemDbContext dbContext, 
        IOptions<ApiOptions> settings, 
        IAmazonS3 s3Client,
        ILoginTokenTool tokenTool)
    {
        _context = dbContext;
        _options = settings.Value;
        _s3Client = s3Client;
        _tokenTool = tokenTool;
    }

    private ObjectResult InvalidTokenResult()
    {
        return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: "Invalid Login Token");
    }

    private ObjectResult NonVendorResult()
    {
        return Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden",
                detail: "Must be a vendor account to add items");
    }

    [HttpGet]
    public IActionResult RootGet()
    {
        return Ok(new { Message = "Hello World from Vendor Root"});
    }
    
    [HttpPost("NewItem")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> PostNewItem([FromForm] NewItemRequest request)
    {
        var isValid = await _tokenTool.VerifyJWTAsync(request.JWT);
        if (!isValid)
        {
            return InvalidTokenResult();
        }

        var cognitoGroups = _tokenTool.GetCognitoGroups(request.JWT);
        if (!cognitoGroups.Contains(_options.VendorGroupName))
        {
            return NonVendorResult();
        }

        var previousItem = 
            _context
                .ItemSet
                .FirstOrDefault(item => item.ItemName.Equals(request.ItemName, StringComparison.CurrentCultureIgnoreCase));

        if (previousItem != null)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict",
                detail: $"Attempting to add '{request.ItemName}', which already exists");
        }


        var file = request.ItemPicture;

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        string itemKey = $"{Guid.NewGuid()}_{file.FileName}";
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
                ItemName = request.ItemName, 
                Price = request.ItemCost, 
                ItemPictureKey = itemKey,
                VendorUserName = _tokenTool.GetUserName(request.JWT)
            });
        _context.SaveChanges();

        return Ok();
    }

    [HttpPost("DeleteItem")]
    public async Task<IActionResult> DeleteItem([FromForm] DeleteRequest request)
    {
        var isValid = await _tokenTool.VerifyJWTAsync(request.JWT);
        if (!isValid)
        {
            return InvalidTokenResult();
        }

        var cognitoGroups = _tokenTool.GetCognitoGroups(request.JWT);
        if (!cognitoGroups.Contains(_options.VendorGroupName))
        {
            return NonVendorResult();
        }

        return Ok();
    }
}