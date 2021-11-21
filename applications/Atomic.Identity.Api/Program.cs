using Atomic.Identity.Api.Data;
using Atomic.Identity.Api.Dtos;
using Atomic.Identity.Api.Localization;
using Atomic.Identity.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
builder.Services.TryAddSingleton<ISystemClock, SystemClock>(); // for security stamp validators in identity system
builder.Services.AddIdentityCore<AppUser>()
    .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
    .AddSignInManager<SignInManager<AppUser>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(IdentityMapperProfile));

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