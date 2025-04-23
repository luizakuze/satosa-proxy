using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Sustainsys.Saml2.AspNetCore2;
using System;
using System.Collections.Generic;

namespace SamlCsharp.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IDictionary<string, string> _satosaEntityIds = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // chave = valor do name="idp" no form
            ["facebook"] = "https://localhost:5002/Mirror/proxy/aHR0cHM6Ly93d3cuZmFjZWJvb2suY29tL2RpYWxvZy9vYXV0aA==",
            ["google"]   = "https://localhost:5002/Mirror/proxy/aHR0cHM6Ly9hY2NvdW50cy5nb29nbGUuY29t"
        };

        [HttpGet("/")]
        public IActionResult Index()
        {
            LimparCookiesSaml();
            return View(); // Views/Home/Index.cshtml
        }

        [HttpPost("/")]
        public IActionResult Index(string? idp)
        {
            LimparCookiesSaml();
            Console.WriteLine($"🔍 idp recebido: {idp}");

            var props = new AuthenticationProperties
            {
                RedirectUri = "/users"
            };

            if (!string.IsNullOrEmpty(idp) && _satosaEntityIds.TryGetValue(idp, out var entityId))
            {
                // injeta o EntityID correto para o Satosa
                props.Items["idp"] = entityId;
            }
            else
            {
                // opcional: você pode lidar com idp inválido aqui
                ModelState.AddModelError("", "Provedor de identidade inválido.");
                return View();
            }

            return Challenge(props, Saml2Defaults.Scheme);
        }

        private void LimparCookiesSaml()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                if (cookie.StartsWith("Saml2.", StringComparison.OrdinalIgnoreCase))
                {
                    Response.Cookies.Delete(cookie);
                }
            }
        }
    }
}
