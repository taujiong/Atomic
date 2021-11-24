using System.Security.Cryptography.X509Certificates;
using Dapr.Client;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Add services to the container

var mvcBuilder = builder.Services.AddRazorPages()
    .AddViewLocalization();
builder.AddAtomicLocalization(mvcBuilder);

var daprClient = new DaprClientBuilder().Build();
builder.Services.AddSingleton(daprClient);

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ExternalScheme)
    .AddQQ(options =>
    {
        options.SignInScheme = IdentityConstants.ExternalScheme;
        options.ClientId = builder.Configuration["OAuth:QQ:ClientId"];
        options.ClientSecret = builder.Configuration["OAuth:QQ:ClientSecret"];
    })
    .AddGitHub(options =>
    {
        options.SignInScheme = IdentityConstants.ExternalScheme;
        options.ClientId = builder.Configuration["OAuth:GitHub:ClientId"];
        options.ClientSecret = builder.Configuration["OAuth:GitHub:ClientSecret"];
    })
    .AddGoogle(options =>
    {
        options.SignInScheme = IdentityConstants.ExternalScheme;
        options.ClientId = builder.Configuration["OAuth:Google:ClientId"];
        options.ClientSecret = builder.Configuration["OAuth:Google:ClientSecret"];
    });

var identityServerBuilder = builder.Services.AddIdentityServer(options =>
{
    options.IssuerUri = builder.Configuration["OAuthServer:IssuerUrl"];
    options.UserInteraction.ErrorUrl = "/Error";
});
var connectionString = builder.Configuration.GetConnectionString("IdentityServer");
identityServerBuilder.AddConfigurationStore<ConfigurationDbContext>(options =>
    {
        options.ConfigureDbContext = b => b.UseNpgsql(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(15), null);
        });
    })
    .AddOperationalStore<PersistedGrantDbContext>(options =>
    {
        options.ConfigureDbContext = b => b.UseNpgsql(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(15), null);
        });
    });

if (builder.Environment.IsDevelopment())
{
    identityServerBuilder.AddDeveloperSigningCredential();
}
else
{
    identityServerBuilder.AddSigningCredential(new X509Certificate2(
        builder.Configuration["OAuthServer:CertPath"],
        builder.Configuration["OAuthServer:CertPassword"]
    ));
}

#endregion

var app = builder.Build();

#region Configure the HTTP request pipeline

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();

app.MapRazorPages();

#endregion

app.Run();