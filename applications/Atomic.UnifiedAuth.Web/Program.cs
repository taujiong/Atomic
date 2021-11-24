using Dapr.Client;
using Microsoft.AspNetCore.Identity;

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

#endregion

app.Run();