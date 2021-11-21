using Atomic.Identity.Api.Data;
using Atomic.Identity.Api.Localization;
using Atomic.Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Add services to the container

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Identity");
    options.UseNpgsql(connectionString, optionsBuilder =>
    {
        optionsBuilder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(15), null);
    });
});
builder.Services.AddDataProtection(); // fot token providers in identity system
builder.Services.AddIdentityCore<AppUser>()
    .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var mvcBuilder = builder.Services.AddControllers();
builder.AddAtomicLocalization(mvcBuilder);
builder.ConfigureApiController();

#endregion

var app = builder.Build();

#region Configure the HTTP request pipeline.

app.UseAtomicControllerPreset();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();