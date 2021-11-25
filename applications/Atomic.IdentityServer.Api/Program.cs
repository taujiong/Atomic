using Atomic.IdentityServer.Api.Data;
using Atomic.IdentityServer.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Add services to the container

builder.AddAtomicCore();

var mvcBuilder = builder.Services.AddControllers();
builder.AddAtomicLocalization(mvcBuilder);
builder.ConfigureApiController();

builder.Services.AddAutoMapper(typeof(IdentityServerMapperProfile));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("IdentityServer");
    options.UseNpgsql(connectionString, optionsBuilder =>
    {
        optionsBuilder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(15), null);
        optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});
builder.Services.AddIdentityServer()
    .AddConfigurationStore<AppDbContext>()
    .AddOperationalStore<AppDbContext>();

#endregion

var app = builder.Build();

#region Configure the HTTP request pipeline.

app.UseAtomicControllerPreset();

app.UseHttpsRedirection();

app.UseAtomicLocalization();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();