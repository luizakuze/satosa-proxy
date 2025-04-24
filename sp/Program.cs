using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using System.Security.Cryptography.X509Certificates;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.Configuration;

        var spFqdn = config["Saml:Sp:Fqdn"];
        var spPort = config["Saml:Sp:Port"];
        var spUrl = $"https://{spFqdn}:{spPort}";
        var encryptionCertName = config["Saml:Sp:Certificates:Encryption"];
        var signingCertName = config["Saml:Sp:Certificates:Signing"];

        // Permitir conexões HTTPS com certificados inválidos (desenvolvimento)
        builder.Services.AddHttpClient("Saml2", client => { })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

        // Autenticação principal
        builder.Services.AddAuthentication(opt =>
        {
            opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = Saml2Defaults.Scheme;
        })
        .AddCookie(opt =>
        {
            opt.Cookie.Name = "Saml2Auth";
            opt.Cookie.SameSite = SameSiteMode.Lax;
            opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        })
        .AddSaml2(opt =>
        {
            opt.SPOptions.Compatibility.IgnoreAuthenticationContextInResponse = true;

            // Ignorar validação de certificado SSL (desenvolvimento)
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            // Configuração do SP
            opt.SPOptions.EntityId = new EntityId($"{spUrl}/Saml2");
            opt.SPOptions.ReturnUrl = new Uri($"{spUrl}/users");
            opt.SPOptions.WantAssertionsSigned = true;
            opt.SPOptions.AuthenticateRequestSigningBehavior = SigningBehavior.Always;

            // Certificados
            var encryptionCert = new X509Certificate2($"Certificates/{encryptionCertName}", "");
            var signingCert = new X509Certificate2($"Certificates/{signingCertName}", "");

            opt.SPOptions.ServiceCertificates.Add(new ServiceCertificate
            {
                Certificate = encryptionCert,
                Use = CertificateUse.Encryption,
                Status = CertificateStatus.Current,
                MetadataPublishOverride = MetadataPublishOverrideType.PublishEncryption
            });

            opt.SPOptions.ServiceCertificates.Add(new ServiceCertificate
            {
                Certificate = signingCert,
                Use = CertificateUse.Signing,
                Status = CertificateStatus.Current,
                MetadataPublishOverride = MetadataPublishOverrideType.PublishSigning
            });

            // Identity Providers (via SATOSA)
            // Facebook
            var fbId = new EntityId(
                "https://localhost:5002/Mirror/proxy/aHR0cHM6Ly93d3cuZmFjZWJvb2suY29tL2RpYWxvZy9vYXV0aA==");

            opt.IdentityProviders.Add(new IdentityProvider(fbId, opt.SPOptions)
            {
                MetadataLocation = "../saml2-social/metadata/frontend_facebook.xml",
                LoadMetadata = true
            });

            // Google
            var googleId = new EntityId(
                "https://localhost:5002/Mirror/proxy/aHR0cHM6Ly9hY2NvdW50cy5nb29nbGUuY29t");

            opt.IdentityProviders.Add(new IdentityProvider(googleId, opt.SPOptions)
            {
                MetadataLocation = "../saml2-social/metadata/frontend_google.xml",
                LoadMetadata = true
            });



        });

        builder.Services.AddControllersWithViews();

        var app = builder.Build();
        // Em ambiente de desenvolvimento, mostre a página de erro detalhada
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            IdentityModelEventSource.ShowPII = true;
        }

        app.UseRouting();
        app.UseCookiePolicy();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapDefaultControllerRoute();
        app.Run(spUrl);

    }
}
