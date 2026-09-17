namespace Api.Utilities;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

public class LoginTokenTool : ILoginTokenTool
{
    private readonly ApiOptions _options;
    
    public LoginTokenTool(ApiOptions options)
    {
        _options = options;
    }

    public async Task<bool> VerifyJWTAsync(string jwt)
    {
        using var httpClient = new HttpClient();
        var jwksUrl = $"https://cognito-idp.{_options.Region}.amazonaws.com/{_options.UserPoolId}/.well-known/jwks.json";
        string jwksJson = await httpClient.GetStringAsync(jwksUrl);
        var jwks = new JsonWebKeySet(jwksJson);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = jwks.GetSigningKeys(),
            RequireSignedTokens = true,

            ValidateIssuer = true,
            ValidIssuer = $"https://cognito-idp.{_options.Region}.amazonaws.com/{_options.UserPoolId}",

            ValidateAudience = false,

            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };

        var handler = new JsonWebTokenHandler();
        TokenValidationResult result = await handler.ValidateTokenAsync(jwt, parameters);

        return result.IsValid;
    }

    public List<string> GetCognitoGroups(string jwt)
    {
        var handler = new JsonWebTokenHandler();
        JsonWebToken token = handler.ReadJsonWebToken(jwt);

        string[] cognitoGroups;
        var hasGroups = token.TryGetPayloadValue("cognito:groups", out cognitoGroups);
        if (!hasGroups)
        {
            return new List<string>();
        }

        return cognitoGroups.ToList();
    }

    public string GetUserName(string jwt)
    {
        var handler = new JsonWebTokenHandler();
        JsonWebToken token = handler.ReadJsonWebToken(jwt);

        string username;
        token.TryGetPayloadValue("username", out username);

        return username;
    }

    
}