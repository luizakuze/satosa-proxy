using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Sustainsys.Saml2.AspNetCore2;

namespace SamlCsharp.Controllers;

[Route("[controller]")]
public class LogoutController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return SignOut(
            new AuthenticationProperties
            {
                RedirectUri = "/"
            },
            CookieAuthenticationDefaults.AuthenticationScheme,
            Saml2Defaults.Scheme
        );
    }
}
