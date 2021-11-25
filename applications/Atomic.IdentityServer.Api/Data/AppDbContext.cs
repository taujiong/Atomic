using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Atomic.IdentityServer.Api.Data;

public class AppDbContext : DbContext, IConfigurationDbContext, IPersistedGrantDbContext
{
    private readonly ConfigurationStoreOptions _configurationStore;
    private readonly OperationalStoreOptions _operationalStore;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ConfigurationStoreOptions configurationStore,
        OperationalStoreOptions operationalStore
    ) : base(options)
    {
        _configurationStore = configurationStore;
        _operationalStore = operationalStore;
    }

    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; } = null!;
    public DbSet<IdentityResource> IdentityResources { get; set; } = null!;
    public DbSet<ApiResource> ApiResources { get; set; } = null!;
    public DbSet<ApiScope> ApiScopes { get; set; } = null!;

    public DbSet<PersistedGrant> PersistedGrants { get; set; } = null!;
    public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; } = null!;

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureClientContext(_configurationStore);
        modelBuilder.ConfigureResourcesContext(_configurationStore);
        modelBuilder.ConfigurePersistedGrantContext(_operationalStore);

        modelBuilder.Entity<Client>(client =>
        {
            client.HasIndex(c => c.ClientName).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}