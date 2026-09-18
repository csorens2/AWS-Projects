namespace Api.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

public static class CommonApiProblems
{
    public static ProblemDetails InvalidToken() => new()
    {
        Title = "Unauthorized",
        Status = StatusCodes.Status401Unauthorized,
        Detail = "Invalid Login Token"
    };

    public static ProblemDetails NonVendor() => new()
    {
        Title = "Forbidden",
        Status = StatusCodes.Status403Forbidden,
        Detail = "Must be a vendor account to add items"
    };

    public static ProblemDetails ItemNotFound(string itemName) => new()
    {
        Title = "Not Found",
        Status = StatusCodes.Status404NotFound,
        Detail = $"Item '{itemName}' not found",
    };
}