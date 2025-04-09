using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace SamlCsharp.Controllers;

[Route("[controller]")]
public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View(); // Views/Home/Index.cshtml
    }

   [HttpPost("/")]
    public IActionResult Index(string? idp)
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = "/users"
        };

        if (!string.IsNullOrEmpty(idp))
        {
            props.Items["idp"] = idp;
        }

        return Challenge(props);
    }

}
