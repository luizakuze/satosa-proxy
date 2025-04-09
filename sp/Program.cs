using Microsoft.AspNetCore.Authentication.Cookies;
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

        _ = builder.Services.AddAuthentication(opt =>
        {
            opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = Saml2Defaults.Scheme;
        })
        .AddCookie(opt =>
        {
            opt.Cookie.SameSite = SameSiteMode.None;
            opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        })
        .AddSaml2(opt =>
        {
            var spEntityId = new EntityId($"{spUrl}/Saml2");
            opt.SPOptions.EntityId = spEntityId;
            opt.SPOptions.WantAssertionsSigned = true;
            opt.SPOptions.AuthenticateRequestSigningBehavior = SigningBehavior.Always;
            opt.SPOptions.ReturnUrl = new Uri($"{spUrl}/users");
 
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


            // --------- Identity Providers --------- 
            // Poderia ter um DiscoveryService para descobrir o IdP
            // e redirecionar para o IdP correto
            opt.IdentityProviders.Add(new IdentityProvider(
                new EntityId("https://localhost:5002/Saml2IDP/google/proxy.xml"), opt.SPOptions)
            {
                LoadMetadata = true,
                MetadataLocation = "https://localhost:5002/Saml2IDP/google/proxy.xml",
                AllowUnsolicitedAuthnResponse = false
            });

            opt.IdentityProviders.Add(new IdentityProvider(
                new EntityId("https://localhost:5002/Saml2IDP/facebook/proxy.xml"), opt.SPOptions)
            {
                LoadMetadata = true,
                MetadataLocation = "https://localhost:5002/Saml2IDP/facebook/proxy.xml",
                AllowUnsolicitedAuthnResponse = false
            });

        });

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        app.UseRouting();
        app.UseCookiePolicy();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapDefaultControllerRoute();
        app.Run();
    }
}
