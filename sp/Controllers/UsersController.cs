using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SamlCsharp.AttributeMaps;

namespace SamlCsharp.Controllers;
 
[Authorize]
[Route("[controller]")]
public class UsersController : Controller
{
    [HttpGet("/users")]
    public IActionResult Index()
    {
        var claims = User.Claims.ToList();

        // Mapeia os claims usando SamlUriMap
        var friendlyClaims = claims
            .Where(c => SamlUriMap.From.ContainsKey(c.Type))
            .GroupBy(c => c.Type)
            .ToDictionary(g => SamlUriMap.From[g.Key], g => g.First().Value);
 
        ViewData["Username"] = friendlyClaims.GetValueOrDefault("eduPersonPrincipalName");
        ViewData["Email"] = friendlyClaims.GetValueOrDefault("mail");
        ViewData["FirstName"] = friendlyClaims.GetValueOrDefault("givenName");
        ViewData["LastName"] = friendlyClaims.GetValueOrDefault("sn");

        // Debug: imprimir todos os claims no console
        // foreach (var claim in claims)
        // {
        //     Console.WriteLine($"CLAIM: {claim.Type} => {claim.Value}");
        // }

        return View(); // Views/Users/Index.cshtml
    }
}


