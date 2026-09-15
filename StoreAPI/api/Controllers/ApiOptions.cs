using System.ComponentModel.DataAnnotations;

public class ApiOptions
{
    [Required]
    public required string ItemPicturesBucketName { get; set; }

    [Required]
    public required string Region { get; set; }

    [Required]
    public required string UserPoolId { get; set; }

    [Required]
    public required string CustomerGroupName {get; set;}

    [Required]
    public required string VendorGroupName {get; set;}
}