namespace Api.Utilities;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface ILoginTokenTool
{
    Task<bool> VerifyJWTAsync(string jwt);
    List<string> GetCognitoGroups(string jwt);
    string GetUsername(string jwt);
}