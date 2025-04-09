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
    public IActionResult Index(string? action)
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/users"
        });
    }


}
