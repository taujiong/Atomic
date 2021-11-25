using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Add services to the container

var mvcBuilder = builder.Services.AddControllers();
builder.AddAtomicLocalization(mvcBuilder);
builder.ConfigureApiController();

var connectionString = builder.Configuration.GetConnectionString("IdentityServer");
var migrationAssembly = typeof(Program).Assembly.GetName().FullName;
builder.Services.AddIdentityServer()
    .AddConfigurationStore<ConfigurationDbContext>(options =>
    {
        options.ConfigureDbContext = b => b.UseNpgsql(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(15), null);
            sqlOptions.MigrationsAssembly(migrationAssembly);
        });
    })
    .AddOperationalStore<PersistedGrantDbContext>(options =>
    {
        options.ConfigureDbContext = b => b.UseNpgsql(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(15), null);
            sqlOptions.MigrationsAssembly(migrationAssembly);
        });
    });

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